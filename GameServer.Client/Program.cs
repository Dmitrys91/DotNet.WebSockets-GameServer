using GameServer.Client;
using GameServer.Common.Clients;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var deviceId = Guid.NewGuid();
var serviceProvider = BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger>();

try
{
    using var gameClient = serviceProvider.GetService<IGameServerClient>()
        ?? throw new NullReferenceException(nameof(IGameServerClient));

    AppDomain.CurrentDomain.ProcessExit += async (sender, args) =>
    {
        await gameClient.DisconnectAsync();
    };

    await gameClient.ConnectAsync("ws://localhost:8080/");

    Console.WriteLine("Welcome to Game Server!" + Environment.NewLine);

    while (true)
    {
        gameClient.StartServerListening();

        gameClient.PrintAvailableCommands();

        var input = Console.ReadKey();

        Console.WriteLine();

        switch (input.Key)
        {
            case ConsoleKey.L:
                await gameClient.LoginAsync(deviceId);
                break;
            case ConsoleKey.U:
                Console.WriteLine($"Please specify, how many coins do you want to add, and press Enter:");
                string inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out var resourceValue))
                {
                    goto default;
                }
                await gameClient.UpdareResourcesAsync(resourceValue);
                break;
            case ConsoleKey.S:
                Console.WriteLine($"Please specify, how many coins do you want to gift, and press Enter:");
                inputString = Console.ReadLine();
                if (!int.TryParse(inputString, out var coinsValue))
                {
                    goto default;
                }
                await gameClient.SendGiftAsync(coinsValue);
                break;
            case ConsoleKey.Q:
                await gameClient.DisconnectAsync();
                return;
            default:
                Console.WriteLine();
                Console.WriteLine("Invalid input");
                break;
        }
    }
}
catch (Exception ex)
{
    logger.Error("An error occured while executing game client demo. {Message}", ex.Message);
    throw;
}

static IServiceProvider BuildServiceProvider()
{
    var serviceProvider = new ServiceCollection()
                   .AddLogging(builder =>
                   {
                       var logger = new LoggerConfiguration()
                                     .MinimumLevel.Debug()
                                     .WriteTo.Debug()
                                     .CreateLogger();

                       builder.AddSerilog(logger);
                   })
                   .AddSingleton<ILogger>(sp =>
                   {
                       return new LoggerConfiguration()
                           .MinimumLevel.Debug()
                           .WriteTo.Debug()
                           .CreateLogger();
                   })
                   .AddTransient<IGameServerClient, GameServerClient>()
                   .BuildServiceProvider();

    return serviceProvider;
}
