using ElizerBot;
using ElizerBot.Adapter;
using ElizerBot.Adapter.Triggers;

using ElizerWork;

using log4net.Config;

using System.Configuration;
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

                var repository = new MemoryMessageRepository();
                var context = new Context(repository);
                await context.Init();
                var updateHandler = new TriggerBasedBotUpdateHandler<Context>(context, 
                    ComponentsBuilder.BuildButtons(),
                    ComponentsBuilder.BuildCommands(),
                    ComponentsBuilder.BuildMessages());
                var bot = updateHandler.BuildAdapter(SupportedMessenger.Telegram, token);
                await bot.Init();
                await bot.SetCommands(new Dictionary<string, string>
                {
                    { "setthischat", "Групповой чат: задать этот чат для заявок" },
                    { "newrequest", "Личный чат: новая заявка" },
                    { "subscribereport", "Личный чат: подписаться на отчеты" },
                    { "unsubscribereport", "Личный чат: отписаться от отчетов" }
                });

                var beatValue = ConfigurationManager.AppSettings["workerBeatSeconds"];
                if (string.IsNullOrEmpty(beatValue) || !int.TryParse(beatValue, out var beatSeconds))
                    beatSeconds = 60;
                var worker = new Worker(TimeSpan.FromSeconds(beatSeconds));
                var dailyReportTask = new TaskWorkItem(DateTime.Now, async () => {
                    var subscribers = context.Settings.ReportSubscribers;
                    if (subscribers != null && subscribers.Any())
                    {
                        var messages = await repository.List().ToListAsync();
                        if (messages.Any())
                        {
                            var reportText = string.Join($"{Environment.NewLine}----{Environment.NewLine}", messages);
                            var fileName = $"Отчет {DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
                            using var memo = new MemoryStream();
                            using var writer = new StreamWriter(memo);
                            await writer.WriteAsync(reportText);
                            await writer.FlushAsync();

                            foreach (var subscriberChat in subscribers)
                            {
                                var chat = new ChatAdapter(subscriberChat, true);
                                var message = new NewMessageAdapter(chat)
                                {
                                    Text = "Отчет",
                                    Attachment = new FileDescriptorAdapter(fileName, () => memo)
                                };
                                await bot.SendMessage(message);
                                await Task.Delay(TimeSpan.FromSeconds(1));
                            }

                            await repository.Clear();
                        }
                    }
                });
                var recurring = new RecurringWorkItem(context.Settings.ReportTime, worker, dailyReportTask, TimeSpan.FromDays(1));
                worker.Queue(recurring);

                Log.WriteInfo("Bot started. Press ^C to stop");
                await worker.Run(cts.Token);
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