namespace GameServer.Common.Models.WebSocket
{
    /// <summary>
    /// Web socket command type (or route) easy switchable
    /// </summary>
    public enum WebSocketCommandType
    {
        Login = 0,
        UpdateResources = 1,
        SendGift = 2,
        Logout = 3
    }
}