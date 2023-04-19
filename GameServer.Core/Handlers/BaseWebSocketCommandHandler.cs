using GameServer.Common.Models.WebSocket;
using GameServer.Core.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using WebSocketMessageType = System.Net.WebSockets.WebSocketMessageType;

namespace GameServer.Core.Handlers
{
    /// <summary>
    /// Base web socket command handler
    /// </summary>
    public abstract class WebSocketCommandHandler
    {
        protected IPlayersRepository _playersRepository;
        protected IConnectionsProdiver _connectionProvider;

        protected WebSocketCommandHandler(IPlayersRepository playersRepository,
            IConnectionsProdiver playersConnectionProvider)
        {
            _playersRepository = playersRepository;
            _connectionProvider = playersConnectionProvider;
        }

        /// <summary>
        /// Handle websocket command
        /// </summary>
        /// <param name="webSocket">websocket</param>
        /// <param name="command">command</param>
        /// <returns></returns>
        public abstract Task HandleAsync(WebSocket webSocket, WebSocketCommand command);

        /// <summary>
        /// Send websocket response
        /// </summary>
        /// <param name="webSocket">socket</param>
        /// <param name="responseObject">response object</param>
        /// <param name="success">success</param>
        /// <param name="notification">notification message</param>
        /// <returns></returns>
        protected async Task SendAsync(WebSocket webSocket, object responseObject, bool success = true, string notification = null)
        {
            var response = new WebSocketResponse
            {
                Success = success, 
                Payload = JObject.FromObject(responseObject),
                NotificationMessage = notification
            };

            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response))),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
