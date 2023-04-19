using GameServer.Core.Abstractions;
using GameServer.Domain;

namespace GameServer.Infrastructure
{
    public class PlayersRepository : IPlayersRepository
    {
        private static readonly List<Player> Players = new()
        {
            new Player() { Id = 1, Coins = 10, Rolls = 10, Nickname = "Panda" },
            new Player() { Id = 2, Coins = 10, Rolls = 10, Nickname = "Panda2"},
            new Player() { Id = 3, Coins = 10, Rolls = 10, Nickname = "Panda3"},
            new Player() { Id = 4, Coins = 10, Rolls = 10, Nickname = "Panda3"},
            new Player() { Id = 5, Coins = 10, Rolls = 10, Nickname = "Panda3"},
            new Player() { Id = 6, Coins = 10, Rolls = 10, Nickname = "Panda3"},
            new Player() { Id = 7, Coins = 10, Rolls = 10, Nickname = "Panda3"},
        };

        public async Task<Player> GetPlayerByDeviceIdAsync(Guid deviceId)
        {
            await Task.Yield();

            var random = new Random().Next(0, Players.Count - 1);

            return Players[random];
        }

        public async Task<Player> GetPlayerByIdAsync(int playerId)
        {
            await Task.Yield();

            return Players.FirstOrDefault(x => x.Id == playerId);
        }

        public async Task<Player> UpdatePlayerAsync(Player player)
        {
            var sourcePlayer = await GetPlayerByIdAsync(player.Id);

            sourcePlayer.Coins = player.Coins;
            sourcePlayer.Rolls = player.Rolls;

            return sourcePlayer;
        }
    }
}
