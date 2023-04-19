using System.Net.WebSockets;

namespace GameServer.Core.Abstractions
{
    public interface IConnectionsProdiver
    {
        /// <summary>
        /// Is player connected
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        Task<bool> IsConnectedAsync(int playerId);

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="playerId"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        Task<bool> ConnectAsync(Guid deviceId, int playerId, WebSocket webSocket);

        /// <summary>
        /// Get player id by websocket
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        Task<int> GetPlayerIdByConnectionSocketAsync(WebSocket webSocket);

        /// <summary>
        /// Get player connection by his id
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        Task<WebSocket> GetPlayerConnectionAsync(int playerId);

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        Task<bool> DisconnectAsync(WebSocket webSocket);
    }
}
