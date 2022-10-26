using Telegram.Bot;
using TelegramBotTest.Logs;
using TelegramBotTest.Utils;

namespace TelegramBotTest
{
    class Program
    {
        private const string TokenVariableName = "token";
        private const string AdminIdVariableName = "admin";

        static async Task Main(string[] args)
        {
            try
            {
                var token = Environment.GetEnvironmentVariable(TokenVariableName);
                if (string.IsNullOrEmpty(token))
                { 
                    await Log.WriteInfo("Token is not set");
                    return;
                }

                var admin = Environment.GetEnvironmentVariable(AdminIdVariableName);
                if (string.IsNullOrEmpty(admin))
                {
                    await Log.WriteInfo("Admin is not set");
                    return;
                }

                if (!long.TryParse(admin, out var adminId))
                {
                    await Log.WriteInfo($"Unable to parse {admin} as admin Id");
                    return;
                }

                var client = new TelegramBotClient(token);
                var bot = new Bot(adminId);
                await bot.Init(client);
                var updateHandler = new UpdateHandler(bot);

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, _) => cts.Cancel();

                await Log.WriteInfo("Bot started. Press ^C to stop");
                await client.ReceiveAsync(updateHandler, cancellationToken: cts.Token);
                await Log.WriteInfo("Bot stopped");
            }
            catch (Exception ex)
            {
                await Log.WriteInfo(ex.GetFullInfo());
                throw;
            }
        }
    }
}