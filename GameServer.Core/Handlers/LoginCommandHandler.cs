using GameServer.Common.Contracts;
using GameServer.Common.Models.WebSocket;
using GameServer.Core.Abstractions;
using GameServer.Core.Extensions.Attributes;
using System.Net.WebSockets;

namespace GameServer.Core.Handlers
{
    [CommandRoute("login")]
    public class LoginCommandHandler : WebSocketCommandHandler
    {
        public LoginCommandHandler(IPlayersRepository playersRepository,
            IConnectionsProdiver playersConnectionProvider): base(playersRepository, playersConnectionProvider) { }

        public override async Task HandleAsync(WebSocket webSocket, WebSocketCommand command)
        {
            var loginRequest = command.Payload.ToObject<LoginRequest>();

            var player = await _playersRepository.GetPlayerByDeviceIdAsync(loginRequest.DeviceId);

            if (await _connectionProvider.IsConnectedAsync(player.Id))
            {
                throw new ApplicationException($"Player with id = {player.Id} already logged in!");
            }

            await _connectionProvider.ConnectAsync(loginRequest.DeviceId, player.Id, webSocket);

            await SendAsync(webSocket, new LoginResponse { PlayerId = player.Id });
        }
    }
}
