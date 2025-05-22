using System.Net.WebSockets;
using Web_test_bot.BotTools;
using Web_test_bot.BotTools.ScriptTools;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем TeleBotClient как синглтон
builder.Services.AddSingleton(provider => 
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("botInst.json", optional: false, reloadOnChange: true)
        .Build();
    
    return new TeleBotClient(configuration);
});

var app = builder.Build();

// Получаем экземпляр бота
var bot = app.Services.GetRequiredService<TeleBotClient>();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

// Регистрируем обработчик завершения приложения
lifetime.ApplicationStopping.Register(() =>
{
    bot.Dispose();
    Console.WriteLine("Application is shutting down...");
});

app.MapGet("/", () => "Hello World!");

app.Run();