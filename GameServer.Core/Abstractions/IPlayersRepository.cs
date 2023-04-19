using GameServer.Domain;

namespace GameServer.Core.Abstractions
{
    public interface IPlayersRepository
    {
        /// <summary>
        /// Get player by id
        /// </summary>
        /// <param name="playerId">player</param>
        /// <returns></returns>
        Task<Player> GetPlayerByIdAsync(int playerId);

        /// <summary>
        /// Get Player by his device id
        /// </summary>
        /// <param name="deviceId">player</param>
        /// <returns></returns>
        Task<Player> GetPlayerByDeviceIdAsync(Guid deviceId);

        /// <summary>
        /// Update player
        /// </summary>
        /// <param name="player">player</param>
        /// <returns></returns>
        Task<Player> UpdatePlayerAsync(Player player);
    }
}
