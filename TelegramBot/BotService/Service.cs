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

            Console.WriteLine($"User: {message.From.Username}, Request: {message.Text}, Date: {DateTime.Now}\n");

            switch (message.Text)
            {
                case "/start":
                    await HandleStartCommand(message, cancellationToken);
                    break;
                case "/questions":
                    await HandleCustomQuestion(message, cancellationToken);
                    break;
                case "/survey":
                    await HandleSurveyCommand(message, cancellationToken);
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
            Console.WriteLine($"Callback Query from User: {update.CallbackQuery.From.Username}, Data: {update.CallbackQuery.Data}, Date: {DateTime.Now}\n");

            await OnCallbackQueryReceived(update.CallbackQuery, cancellationToken);
        }
    }

    private async Task HandleStartCommand(Message message, CancellationToken cancellationToken)
    {
        await SendWelcomeMessageAsync(message.Chat.Id, cancellationToken);
    }
    private async Task SendWelcomeMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        var welcomeMessage = "Вас вітає система на базі Єдиного державного реєстру ветеранів війни. " +
                                "Можете скористатися наступними командами, щоб комунікувати зі мною \n" +
                                "\n /questions - запитання на які я можу дати відповідь" +
                                "\n /contact - зв'язок із нашим спеціалістом" +
                                "\n /survey - коротке опитування, яке допоможе нам краще зрозуміти вас";
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
        var callbackData = callbackQuery.Data.Split('_');
        if (callbackData.Length == 3 && (callbackData[0] == "answer"))
        {
            // Обробка відповіді на питання опитування
            await HandleSurveyResponse(callbackQuery, int.Parse(callbackData[2]), cancellationToken);
        }
        else
        {
            switch (callbackQuery.Data)
            {
                case "benefits":
                    await HandleBenefits(callbackQuery, cancellationToken);
                    break;
                case "statuses":
                    await HandleStatuses(callbackQuery, cancellationToken);
                    break;
                // інші випадки обробки зворотних викликів
            }
        }
    }
    private async Task HandleSurveyResponse(CallbackQuery callbackQuery, int questionIndex, CancellationToken cancellationToken)
    {
        var thankYouMessage = "Дякую за відповідь.";

        await botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message.Chat.Id,
            text: thankYouMessage,
            cancellationToken: cancellationToken
        );

        await SendSurveyAsync(callbackQuery.Message.Chat.Id, questionIndex + 1, cancellationToken);
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

    private async Task HandleSurveyCommand(Message message, CancellationToken cancellationToken)
    {
        await SendSurveyAsync(message.Chat.Id, 0, cancellationToken);
    }

    private async Task SendSurveyAsync(long chatId, int questionIndex, CancellationToken cancellationToken)
    {
        var questions = new List<string>
        {
            "Як ви себе почуваєте у суспільстві?",
            "Як ви себе почуваєте у людних місцях?",
            "Як ви оцінюєте свій настрій?"
        };
        
        if (questionIndex >= questions.Count)
        {
            await botClient.SendTextMessageAsync(chatId, "Дякуємо за участь у опитуванні, ваші відповіді допоможуть нашим спеціалістам краще зрозуміти вас !", cancellationToken: cancellationToken);
            return;
        }

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[] // перший ряд кнопок
            {
                InlineKeyboardButton.WithCallbackData("Погано", $"answer_bad_{questionIndex}"),
                InlineKeyboardButton.WithCallbackData("Добре", $"answer_good_{questionIndex}"),
                InlineKeyboardButton.WithCallbackData("Відмінно", $"answer_excellent_{questionIndex}")
            },
            
        });

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: questions[questionIndex],
            replyMarkup: inlineKeyboard,
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
                                       $"Будь ласка, зверніться до нашого спеціаліста: {AdminUrl} " +
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


