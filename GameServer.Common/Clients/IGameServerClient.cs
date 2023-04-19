using GameServer.Common.Models.WebSocket;

namespace GameServer.Common.Clients
{
    /// <summary>
    /// Game client
    /// </summary>
    public interface IGameServerClient : IDisposable
    {
        /// <summary>
        /// Returns value if user is logged in
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="webSocketServerUri">url</param>
        /// <returns></returns>
        Task ConnectAsync(string webSocketServerUri);

        /// <summary>
        /// Disconnect from server
        /// </summary>
        /// <returns></returns>
        Task DisconnectAsync();

        /// <summary>
        /// Send command to server
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="onMessageReceived">on recieved event handler</param>
        /// <returns></returns>
        Task SendAsync(WebSocketCommand command, Action<WebSocketResponse> onMessageReceived = null);

        /// <summary>
        /// Login within device id
        /// </summary>
        /// <param name="deviceId">device id</param>
        /// <returns></returns>
        Task LoginAsync(Guid deviceId);

        /// <summary>
        /// Update resources
        /// </summary>
        /// <param name="resourceValue">resource value</param>
        /// <returns></returns>
        Task UpdareResourcesAsync(int resourceValue);

        /// <summary>
        /// Send gift (coins)
        /// </summary>
        /// <param name="resourceValue">resource value</param>
        /// <returns></returns>
        Task SendGiftAsync(int resourceValue);

        /// <summary>
        /// Start listening game server
        /// </summary>
        void StartServerListening();

        /// <summary>
        /// Shows available game client commands
        /// </summary>
        void PrintAvailableCommands();
    }
}
