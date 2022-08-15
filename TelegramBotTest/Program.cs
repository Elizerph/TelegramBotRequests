using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var token = Environment.GetEnvironmentVariable("requestBotToken");
            if (string.IsNullOrEmpty(token))
                return;

            var client = new TelegramBotClient(token);

            await client.DeleteMyCommandsAsync();
            var commands = new[]
            {
                new BotCommand { Command = UpdateHandler.SetChatCommand, Description = "Chat for requests to post" },
                new BotCommand { Command = UpdateHandler.NewRequestCommand, Description = "Creates new request" }
            };
            await client.SetMyCommandsAsync(commands);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, _) => cts.Cancel();
            var updateHandler = new UpdateHandler();
            await updateHandler.Init();
            client.StartReceiving(updateHandler, new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } }, cts.Token);

            Console.WriteLine("Bot started. Press ^C to stop");
            await Task.Delay(-1, cancellationToken: cts.Token);
            Console.WriteLine("Bot stopped");
        }
    }
}