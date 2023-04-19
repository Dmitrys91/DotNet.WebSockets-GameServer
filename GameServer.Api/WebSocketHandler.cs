using GameServer.Common.Models.WebSocket;
using GameServer.Core.Abstractions;
using GameServer.Core.Extensions.Attributes;
using GameServer.Core.Handlers;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;

namespace GameServer.Api
{
    /// <summary>
    /// General socket handler
    /// </summary>
    public interface IWebSocketHandler
    {
        /// <summary>
        /// Handle socket connection
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        Task WebSocketWaitLoopAsync(WebSocket webSocket);
    }

    public class WebSocketHandler : IWebSocketHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionsProdiver _connectionProvider;
        private readonly ILogger<WebSocketHandler> _logger;

        private readonly Dictionary<string, Type> _routingHandlersTable = RegisterRoutes();

        private const int bufferSize = 4096;

        public WebSocketHandler(IServiceProvider serviceProvider,
            IConnectionsProdiver playersConnectionProvider,
            ILogger<WebSocketHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _connectionProvider = playersConnectionProvider;
            _logger = logger;
        }

        public async Task WebSocketWaitLoopAsync(WebSocket webSocket)
        {
            while (webSocket.State.HasFlag(WebSocketState.Open))
            {
                var buffer = WebSocket.CreateServerBuffer(bufferSize);

                try
                {
                    var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                    if (!string.IsNullOrEmpty(message))
                    {
                        var command = JsonConvert.DeserializeObject<WebSocketCommand>(message);

                        var commandHandler = GetCommandHandlerByRoute(command);
                        if (commandHandler != null)
                        {
                            _logger.LogInformation("Starting execution for command: {Route}", command.Route);

                            await commandHandler.HandleAsync(webSocket, command);
                        }
                        else
                        {
                            _logger.LogError("Unknown command or route. command: {Route}", command.Route);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is not WebSocketException)
                    {
                        await HandleCommandExceptionAsync(webSocket, ex);
                    }

                    _logger.LogError("An error occured while processing websocket. Message: {Message}", ex.Message); 
                }
            }

            await CloseAsync(webSocket);
        }

        /// <summary>
        /// Close connection 
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        private async Task CloseAsync(WebSocket webSocket)
        {
            if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
            {
                try
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                        "Socket closed", CancellationToken.None);

                    _logger.LogError("WebSocket connection closed");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occured while closing websocket connection. Message: {Message}", ex.Message);
                }
                finally
                {
                    await _connectionProvider.DisconnectAsync(webSocket);
                }
            }
        }

        /// <summary>
        /// Handle command exception
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private async Task HandleCommandExceptionAsync(WebSocket webSocket, Exception ex)
        {
            if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
            {
                var response = new WebSocketResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };

                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response))),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        /// <summary>
        /// Get command handler
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private WebSocketCommandHandler GetCommandHandlerByRoute(WebSocketCommand command)
        {
            if (_routingHandlersTable.TryGetValue(command.Route, out var handlerType))
            {
                var handler = _serviceProvider.GetService(handlerType) as WebSocketCommandHandler;
                return handler;
            }

            return null;
        }

        /// <summary>
        /// Register table routes
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static Dictionary<string, Type> RegisterRoutes()
        {
            var routeTable = new Dictionary<string, Type>();

            var asmNames = DependencyContext.Default.GetDefaultAssemblyNames();

            var type = typeof(WebSocketCommandHandler);

            var handlers = asmNames.Select(Assembly.Load)
                .SelectMany(t => t.GetTypes())
                .Where(p => p.GetTypeInfo().IsSubclassOf(type) && p != null);

            foreach (var handlerType in handlers)
            {
                var routeAttribute = handlerType.GetCustomAttribute<CommandRouteAttribute>();
                if (routeAttribute is null || string.IsNullOrWhiteSpace(routeAttribute.Route))
                {
                    throw new ArgumentNullException($"Command {handlerType.FullName} handler does not have route attribute");
                }

                if (routeTable.ContainsKey(routeAttribute.Route))
                {
                    throw new ArgumentNullException($"Duplicated routes. Command: {handlerType.FullName}");
                }

                routeTable.Add(routeAttribute.Route, handlerType);
            }

            return routeTable;
        }
    }
}

