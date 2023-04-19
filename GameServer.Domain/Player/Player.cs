namespace GameServer.Domain
{
    public class Player
    {
        /// <summary>
        /// Player id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Player nickname
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Player coins
        /// </summary>
        public int Coins { get; set; }

        /// <summary>
        /// Player rolls
        /// </summary>
        public int Rolls { get; set; }
    }
}
