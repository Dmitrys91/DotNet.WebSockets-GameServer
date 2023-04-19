using GameServer.Common.Clients;
using GameServer.Common.Contracts;
using GameServer.Common.Models.WebSocket;
using GameServer.Domain;
using Newtonsoft.Json;
using Serilog;
using System.Net.WebSockets;
using System.Text;

namespace GameServer.Client
{
    public class GameServerClient : IGameServerClient
    {
        private ClientWebSocket _webSocket;
        private Action<WebSocketResponse>? OnMessageRecieved = null;
        private int? _playerId;

        private readonly ILogger _logger;

        public GameServerClient(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsLoggedIn => _webSocket != null && _playerId.HasValue;

        public bool HasListener => OnMessageRecieved != null;

        public async Task ConnectAsync(string webSocketServerUri)
        {
            _webSocket = new ClientWebSocket();

            await _webSocket.ConnectAsync(new Uri(webSocketServerUri), CancellationToken.None);

            if (_webSocket.State == WebSocketState.Open)
            {
                _logger.Debug("Successfully connected to server: {webSocketServerUri}", webSocketServerUri);
            }
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket.State != WebSocketState.Closed && _webSocket.State != WebSocketState.Aborted)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                  "Closing connection", CancellationToken.None);
            }
        }

        public async Task SendAsync(WebSocketCommand command, Action<WebSocketResponse>? onMessageReceived = null)
        {
            try
            {
                OnMessageRecieved = onMessageReceived;

                var json = JsonConvert.SerializeObject(command);

                _logger.Debug("Sending command: {Route}", command.Route);

                await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occured while sending websocket. user id: {_playerId}, command: {Route}. Message: {Message}",
                    _playerId, command.Route, ex.Message);
            }
        }

        private void HandleWebSocketResponse(WebSocketResponse response)
        {
            if (response is null)
                return;

            try
            {
                _logger.Debug("Recieved the response with status: {Success}", response.Success);

                if (!response.Success)
                {
                    Console.WriteLine(response.ErrorMessage);
                    return;
                }

                if (OnMessageRecieved != null)
                {
                    OnMessageRecieved(response);
                    OnMessageRecieved = null;
                }
                else
                {
                    Console.WriteLine(response.NotificationMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("An error occured while receiveing websocket.  Message: {Message}", ex.Message);
            }
        }

        public void StartServerListening()
        {
            _ = Task.Run(async () =>
            {
                _logger.Debug("Starting listening...");

                try
                {
                    var buffer = WebSocket.CreateServerBuffer(4096);
                    var item = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    var responseJson = Encoding.UTF8.GetString(buffer.Array, 0, item.Count);
                    var responseMessage = JsonConvert.DeserializeObject<WebSocketResponse>(responseJson);

                    HandleWebSocketResponse(responseMessage);
                }
                catch (Exception ex)
                {
                    _logger.Error("An error occured while listening the server. Message: {Message}", ex.Message);
                }
            });
        }

        public async Task LoginAsync(Guid deviceId)
        {
            if (_playerId.HasValue)
            {
                Console.WriteLine($"Aready logged in with ID: {_playerId}");
                return;
            }

            var loginCommand = new WebSocketCommand("login", new LoginRequest { DeviceId = deviceId });

            await SendAsync(loginCommand, (response) =>
            {
                var loginResponse = response.Payload.ToObject<LoginResponse>();
                _playerId = loginResponse?.PlayerId;
                Console.WriteLine();
                Console.WriteLine($"Player logged in with ID: {loginResponse.PlayerId}");
            });
        }

        public async Task UpdareResourcesAsync(int resourceValue)
        {
            var updateResourcesCommand = new WebSocketCommand("update-resources",
                new UpdateResourceRequest { ResourceType = ResourceType.Coins, ResourceValue = resourceValue });

            await SendAsync(updateResourcesCommand, (response) => {
                var updateResourceResponse = response.Payload.ToObject<UpdateResourceResponse>();
                Console.WriteLine();
                Console.WriteLine($"Player resources updated. New balance is: {updateResourceResponse?.ResourceValue} coins");
            });
        }

        public async Task SendGiftAsync(int resourceValue)
        {
            var sendGiftCommand = new WebSocketCommand("send-gift",
                new SendGiftRequest { FriendPlayerId = 2, ResourceType = ResourceType.Coins, ResourceValue = resourceValue });

            await SendAsync(sendGiftCommand, (response) => {
                Console.WriteLine();
                Console.WriteLine($"Gift sent successfully!");
            });
        }

        public void PrintAvailableCommands()
        {
            if (!IsLoggedIn && !HasListener)
            {
                Console.WriteLine();
                Console.WriteLine("Available commands:");
                Console.WriteLine("press 'l' - login to the system");
                Console.WriteLine("press 's' - send the gift");
                Console.WriteLine("press 'u' - update resources");
                Console.WriteLine("press 'q' - quit from the system");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_webSocket.State != WebSocketState.Closed && _webSocket.State != WebSocketState.Aborted)
                {
                    _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                      "Closing connection", CancellationToken.None).GetAwaiter().GetResult();
                }
            }
        }
    }
}
