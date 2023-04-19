using GameServer.Core.Abstractions;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace GameServer.Infrastructure
{
    public class PlayersConnectionProvider : IConnectionsProdiver
    {
        private static readonly ConcurrentDictionary<WebSocket, PlayerConnection> connections = new();

        public async Task<bool> ConnectAsync(Guid deviceId, int playerId, WebSocket webSocket)
        {
            await Task.Yield();

            return connections.TryAdd(webSocket, new PlayerConnection(deviceId, playerId));
        }

        public async Task<bool> IsConnectedAsync(int playerId)
        {
            await Task.Yield();

            return connections.Any(x => x.Value.PlayerId == playerId);
        }

        public async Task<int> GetPlayerIdByConnectionSocketAsync(WebSocket webSocket)
        {
            await Task.Yield();

            if (connections.TryGetValue(webSocket, out var connection))
            {
                return connection.PlayerId;
            }

            return default;
        }

        public async Task<WebSocket> GetPlayerConnectionAsync(int playerId)
        {
            await Task.Yield();

            var connection = connections.FirstOrDefault(x=> x.Value.PlayerId == playerId);
            if (connection.Value != null)
                return connection.Key;

            return null;
        }

        public async Task<bool> DisconnectAsync(WebSocket webSocket)
        {
            await Task.Yield();

            return connections.TryRemove(webSocket, out _);
        }
    }

    /// <summary>
    /// Player connection state
    /// </summary>
    public class PlayerConnection
    {
        internal PlayerConnection(Guid deviceId, int playerId)
        {
            DeviceId = deviceId;
            PlayerId = playerId;
        }

        /// <summary>
        /// Device id
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// Player Id
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// Player web socket (also make sence to place socket id)
        /// </summary>
        public WebSocket WebSocket { get; set; }
    }
}
