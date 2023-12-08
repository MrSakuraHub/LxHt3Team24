using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace TelegramBot.BotService;

public class BotService
{
    private readonly ITelegramBotClient botClient;
    private readonly CancellationTokenSource cts;
    private const string AdminUrl = "https://example.com/contact-admin"; // URL сторінки адміністратора


    public BotService(string token)
    {
        botClient = new TelegramBotClient(token);
        cts = new CancellationTokenSource();
    }

    public async Task StartAsync()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Receive all update types
        };
        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cts.Token
        );

        Console.WriteLine("Bot started.");
    }

    public void Stop()
    {
        cts.Cancel();
        Console.WriteLine("Bot stopped.");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var message = update.Message;

            switch (message.Text)
            {
                case "/start":
                    await HandleStartCommand(message, cancellationToken);
                    break;
                case "/questions":
                    await HandleCustomQuestion(message, cancellationToken);
                    break;
                case "/contact":
                    await HandleContactSpecialist(message, cancellationToken);
                    break;
                default:
                    await DefaultHandleAsync(message, cancellationToken);
                    break;
            }
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            await OnCallbackQueryReceived(update.CallbackQuery, cancellationToken);
        }
    }

    private async Task HandleStartCommand(Message message, CancellationToken cancellationToken)
    {
        await SendWelcomeMessageAsync(message.Chat.Id, cancellationToken);
    }
    private async Task SendWelcomeMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        string welcomeMessage = "Вас вітає система на базі Єдиного державного реєстру ветеранів війни. " +
                                "Можете скористатися наступними командами, щоб комунікувати зі мною \n" +
                                "\n /questions - запитання на які я можу дати відповідь" +
                                "\n /contact - зв'язок із нашим спеціалістом";
        await botClient.SendTextMessageAsync(chatId, welcomeMessage, cancellationToken: cancellationToken);
    }

    private async Task HandleCustomQuestion(Message message, CancellationToken cancellationToken)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[] // перший ряд кнопок
            {
                InlineKeyboardButton.WithCallbackData("Гарантії та пільги", "benefits"),
                InlineKeyboardButton.WithCallbackData("Статуси", "statuses")
            },
        });

        await botClient.SendTextMessageAsync(
            chatId: message.Chat,
            text: "Будь ласка, виберіть один з доступних варіантів:",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken
        );
    }
    private async Task HandleContactSpecialist(Message message, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
            chatId: message.Chat,
            text: $"контакти нашого спеціаліста: {AdminUrl}",
            cancellationToken: cancellationToken
        );
    }

    public async Task OnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        switch (callbackQuery.Data)
        {
            case "benefits":
                await HandleBenefits(callbackQuery, cancellationToken);
                break;
            case "statuses":
                await HandleStatuses(callbackQuery, cancellationToken);
                break;
        }
    }

    private async Task HandleStatuses(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message.Chat,
            text: $"Тут ви можете дізнатись про надання статусів учасника бойових дій, особи з інвалідністю внаслідок війни," +
                  " учасника війни, члена сім’ї загиблого: {https://eveteran.gov.ua/statuses}", 
            cancellationToken: cancellationToken
        );
    }

    private async Task HandleBenefits(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message.Chat,
            text: $"Тут ви можете дізнатись про пільги на проїзд, навчання, комунальні послуги для осіб з інвалідністю," +
                  " учасників бойових дій, членів сімей: {https://eveteran.gov.ua/benefits}",
            cancellationToken: cancellationToken
        );
    }



    private async Task DefaultHandleAsync(Message message, CancellationToken cancellationToken)
    {
        await SendUnknownCommandMessageAsync(message.Chat.Id, cancellationToken);
    }

    private async Task SendUnknownCommandMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        string unknownCommandMessage = $"На жаль, у мене немає відповіді на ваше питання. " +
                                       "Будь ласка, зверніться до нашого спеціаліста: {AdminUrl} " +
                                       "\n\nВи також можете спробувати ці команди: \n/start \n/questions \n/contact";
        await botClient.SendTextMessageAsync(chatId, unknownCommandMessage, cancellationToken: cancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Логування помилки
        Console.WriteLine($"An error occurred: {exception.Message}");
        
        return Task.CompletedTask; 
    }
}


