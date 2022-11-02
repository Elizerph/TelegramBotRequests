using log4net.Config;

using System.Reflection;

using Telegram.Bot;

namespace TelegramBotTest
{
    class Program
    {
        private const string TokenVariableName = "token";

        static async Task Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure(); 
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyVersion = assembly.GetName().Version;
                Log.WriteInfo($"Version {assemblyVersion}");

                var token = Environment.GetEnvironmentVariable(TokenVariableName);
                if (string.IsNullOrEmpty(token))
                { 
                    Log.WriteInfo("Token is not set");
                    return;
                }

                var client = new TelegramBotClient(token);
                var bot = new Bot();
                await bot.Init(client);
                var updateHandler = new UpdateHandler(bot);

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, _) => cts.Cancel();

                Log.WriteInfo("Bot started. Press ^C to stop");
                await client.ReceiveAsync(updateHandler, cancellationToken: cts.Token);
                Log.WriteInfo("Bot stopped");
            }
            catch (Exception ex)
            {
                Log.WriteInfo("Generic exception", ex);
                throw;
            }
        }
    }
}