using Telegram.Bot;

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
            var bot = new Bot();
            await bot.Init(client);
            var updateHandler = new UpdateHandler(bot);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, _) => cts.Cancel();

            Console.WriteLine("Bot started. Press ^C to stop"); 
            await client.ReceiveAsync(updateHandler, cancellationToken: cts.Token);
            Console.WriteLine("Bot stopped");
        }
    }
}