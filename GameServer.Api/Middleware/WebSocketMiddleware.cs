using System.Diagnostics;

namespace GameServer.Api.Middleware
{
    public class WebSocketMiddleware : IMiddleware
    {
        private IServiceProvider _serviceProvider;
        private ILogger<WebSocketMiddleware> _logger;

        public WebSocketMiddleware(IServiceProvider serviceProvider,
            ILogger<WebSocketMiddleware> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    var webSocketHandler = _serviceProvider.GetService<IWebSocketHandler>();
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    await webSocketHandler.WebSocketWaitLoopAsync(webSocket);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    _logger.LogError("An error occured while handling socket messaging. {Message}", ex.Message);
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}

