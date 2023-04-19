using Newtonsoft.Json.Linq;

namespace GameServer.Common.Models.WebSocket
{
    /// <summary>
    /// General web socket model with payload
    /// </summary>
    public class WebSocketCommand
    {
        public WebSocketCommand(string route, object payload)
        {
            Route = route;
            Payload = JObject.FromObject(payload);
        }

        /// <summary>
        /// Web socket command type (alternative for routing)
        /// </summary>
        public WebSocketCommandType Type { get; set; }

        /// <summary>
        /// Web socket command payload
        /// </summary>
        public JObject Payload { get; set; }

        /// <summary>
        /// Command route
        /// </summary>
        public string Route { get; set; }
    }
}
