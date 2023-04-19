using GameServer.Api.Middleware;
using GameServer.Core.Abstractions;
using GameServer.Core.Handlers;
using GameServer.Infrastructure;
using Microsoft.Extensions.DependencyModel;
using Serilog;
using System.Reflection;

namespace GameServer.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        /// <summary>
        /// Add logging
        /// </summary>
        /// <param name="host"></param>
        public static void AddLogging(this ConfigureHostBuilder host) 
        {
            host.UseSerilog((context, configuration) => configuration
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Debug());

        }

        /// <summary>
        /// Add dependency injection
        /// </summary>
        /// <param name="services"></param>
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<IPlayersRepository, PlayersRepository>();
            services.AddTransient<IConnectionsProdiver, PlayersConnectionProvider>();
            services.AddTransient<IWebSocketHandler, WebSocketHandler>();
            services.AddTransient<WebSocketMiddleware>();
            services.AddCommandHandlers();
        }

        /// <summary>
        /// Register all websocket command handlers
        /// </summary>
        /// <param name="services"></param>
        private static void AddCommandHandlers(this IServiceCollection services)
        {
            var asmNames = DependencyContext.Default.GetDefaultAssemblyNames();

            var type = typeof(WebSocketCommandHandler);

            var handlers = asmNames.Select(Assembly.Load)
                .SelectMany(t => t.GetTypes())
                .Where(p => p.GetTypeInfo().IsSubclassOf(type) && p != null);

            foreach (var commandHandler in handlers)
            {
                services.AddTransient(commandHandler);
            }
        }
    }
}
