using GameServer.Domain;

namespace GameServer.Common.Contracts
{
    public class UpdateResourceResponse
    {
        public ResourceType ResourceType { get; set; }

        public int ResourceValue { get; set; }
    }
}
