using GameServer.Domain;

namespace GameServer.Common.Contracts
{
    public class UpdateResourceRequest
    {
        public int ResourceValue { get; set; }

        public ResourceType ResourceType { get; set; }
    }
}
