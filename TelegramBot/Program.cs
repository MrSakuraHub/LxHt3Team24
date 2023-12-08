using Telegram.Bot;
using TelegramBot.BotService;

namespace TelegramBot;

class Program
{
    static async Task Main(string[] args)
    {
        var token = "6744683848:AAHieetmjaIzxfo0QzFOM6vA1McMUz9sqzY"; // Ваш токен
        var botService = new BotService.BotService(token);

        await botService.StartAsync();

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        botService.Stop();
    }
}