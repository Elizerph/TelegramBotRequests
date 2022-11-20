using ElizerBot;
using ElizerBot.Adapter;
using ElizerBot.Adapter.Triggers;

using log4net.Config;

using System.Reflection;

using TelegramBotTest.Components;

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

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, _) => cts.Cancel();

                Log.WriteInfo("Bot started. Press ^C to stop");
                var context = new Context();
                await context.Init();
                var updateHandler = new TriggerBasedBotUpdateHandler<Context>(context, 
                    ComponentsBuilder.BuildButtons(), 
                    ComponentsBuilder.BuildCommands(), 
                    ComponentsBuilder.BuildMessages());
                var bot = updateHandler.BuildAdapter(SupportedMessenger.Telegram, token);
                await bot.Init();
                await bot.SetCommands(new Dictionary<string, string>
                {
                    { "setthischat", "Задать этот чат для заявок" },
                    { "newrequest", "Новая заявка" }
                });
                await Task.Delay(-1, cts.Token);

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