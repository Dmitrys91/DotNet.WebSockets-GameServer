using GameServer.Domain;

namespace GameServer.Common.Contracts
{
    public class GiftResponse
    {
        public int FromPlayerId { get; set; }

        public int ResourceValue { get; set; }

        public ResourceType ResourceType { get; set; }
    }
}
