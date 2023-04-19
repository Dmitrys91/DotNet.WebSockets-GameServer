using GameServer.Common.Contracts;
using GameServer.Common.Models.WebSocket;
using GameServer.Core.Abstractions;
using GameServer.Core.Extensions.Attributes;
using System.Net.WebSockets;

namespace GameServer.Core.Handlers
{
    [CommandRoute("send-gift")]
    public class SendGiftCommandHandler : WebSocketCommandHandler
    {
        public SendGiftCommandHandler(IPlayersRepository playersRepository,
            IConnectionsProdiver playersConnectionProvider) :  base(playersRepository, playersConnectionProvider) { }

        public override async Task HandleAsync(WebSocket webSocket, WebSocketCommand command)
        {
            var currentUserId = await _connectionProvider.GetPlayerIdByConnectionSocketAsync(webSocket);

            var sendGiftRequest = command.Payload.ToObject<SendGiftRequest>()
                 ?? throw new NullReferenceException("Invalid request");

            if (sendGiftRequest.ResourceValue <= 0)
            {
                throw new ArgumentException($"Gift can not be less then at least 1 coin. Value: {sendGiftRequest.ResourceValue}");
            }

            var receiver = await _playersRepository.GetPlayerByIdAsync(sendGiftRequest.FriendPlayerId) 
                ?? throw new NullReferenceException($"Friend with id = {sendGiftRequest.FriendPlayerId} not found");
            
            var response = new SendGiftResponse();

            await SendAsync(webSocket, response);

#if DEBUG
            // send it to myself for debug

            _ = Task.Run(async () => {
                await Task.Delay(5000);

                var giftToMyself = new GiftResponse
                {
                    FromPlayerId = currentUserId,
                    ResourceType = sendGiftRequest.ResourceType,
                    ResourceValue = sendGiftRequest.ResourceValue,
                };

                await SendAsync(webSocket, giftToMyself, true,
                    $"Congragulations! You obtained {sendGiftRequest.ResourceValue} {sendGiftRequest.ResourceType} as a gift!");
            });
            
#endif

            if (await _connectionProvider.IsConnectedAsync(receiver.Id))
            {
                var friendConnection = await _connectionProvider.GetPlayerConnectionAsync(receiver.Id);

                var giftResponse = new GiftResponse
                {
                    FromPlayerId = currentUserId,
                    ResourceType = sendGiftRequest.ResourceType,
                    ResourceValue = sendGiftRequest.ResourceValue,
                };

                await SendAsync(friendConnection, giftResponse, true,
                    $"Congragulations! You obtained {sendGiftRequest.ResourceValue} {sendGiftRequest.ResourceType} as a gift!");
            }
        }
    }
}
