using GameServer.Common.Contracts;
using GameServer.Common.Models.WebSocket;
using GameServer.Core.Abstractions;
using GameServer.Core.Extensions.Attributes;
using GameServer.Domain;
using System.Net.WebSockets;

namespace GameServer.Core.Handlers
{
    [CommandRoute("update-resources")]
    public class UpdateResourcesCommandHandler : WebSocketCommandHandler
    {
        public UpdateResourcesCommandHandler(IPlayersRepository playersRepository,
            IConnectionsProdiver playersConnectionProvider) : base(playersRepository, playersConnectionProvider) { }

        public override async Task HandleAsync(WebSocket webSocket, WebSocketCommand command)
        {
            var resourceRequest = command.Payload.ToObject<UpdateResourceRequest>()
                ?? throw new NullReferenceException("Invalid request");

            var playerId = await _connectionProvider.GetPlayerIdByConnectionSocketAsync(webSocket);
            if (playerId == default)
                return;

            if (resourceRequest.ResourceValue < 0)
            {
                throw new ArgumentException($"Balance can not be less than zero. Value: {resourceRequest.ResourceValue}");
            }

            var player = await _playersRepository.GetPlayerByIdAsync(playerId);

            switch (resourceRequest.ResourceType)
            {
                case ResourceType.Rolls:
                    player.Rolls += resourceRequest.ResourceValue;
                    break;
                case ResourceType.Coins:
                    player.Coins += resourceRequest.ResourceValue;
                    break;
            }

            await _playersRepository.UpdatePlayerAsync(player);

            var socketResponse = new UpdateResourceResponse
            {
                ResourceType = resourceRequest.ResourceType,
                ResourceValue = resourceRequest.ResourceType == ResourceType.Rolls ? player.Rolls : player.Coins
            };

            await SendAsync(webSocket, socketResponse);
        }
    }
}
