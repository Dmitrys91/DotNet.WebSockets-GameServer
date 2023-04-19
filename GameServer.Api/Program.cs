using GameServer.Api.Extensions;
using GameServer.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencyInjection();
builder.Host.AddLogging();

var app = builder.Build();

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.Run("http://localhost:8080/");


