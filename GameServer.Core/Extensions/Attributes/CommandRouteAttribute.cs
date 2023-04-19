namespace GameServer.Core.Extensions.Attributes
{
    /// <summary>
    /// Command route attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandRouteAttribute : Attribute
    {
        public CommandRouteAttribute(string route) => Route = route;

        /// <summary>
        /// Command route
        /// </summary>
        /// <param name="route"></param>
        public string Route { get; }
    }
}
