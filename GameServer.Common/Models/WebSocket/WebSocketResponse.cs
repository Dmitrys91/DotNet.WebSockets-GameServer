using Newtonsoft.Json.Linq;

namespace GameServer.Common.Models.WebSocket
{
    /// <summary>
    /// Web socket message response model
    /// </summary>
    public class WebSocketResponse
    {
        /// <summary>
        /// Returns value if response has success state
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Response main payload
        /// </summary>
        public JObject Payload { get; set; }

        /// <summary>
        /// Notification message
        /// </summary>
        public string NotificationMessage { get; set; }
    }
}
