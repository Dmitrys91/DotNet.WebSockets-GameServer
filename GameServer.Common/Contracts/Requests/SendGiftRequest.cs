using GameServer.Domain;

namespace GameServer.Common.Contracts
{
    public class SendGiftRequest
    {
        public int FriendPlayerId { get; set; }

        public int ResourceValue { get; set; }

        public ResourceType ResourceType { get; set; }
    }
}
