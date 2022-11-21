using TelegramBotTest.Utils;

namespace TelegramBotTest.Components
{
    public class Context
    {
        public IMessageRepository Repository { get; }

        private const string SettingsFile = "Data/Settings.json";
        private const string TemplateFile = "Data/Template.json";

        public Dictionary<string, Ticket> EditingTickets { get; } = new();
        public SettingsSet? Settings { get; set; }
        public TicketTemplate? Template { get; set; }

        public Context(IMessageRepository repository)
        {
            Repository = repository;
        }

        public async Task Init()
        {
            Log.WriteInfo("Initialization…");
            Settings = await TryRead("Settings", SettingsFile, SettingsSet.Default);
            Template = await TryRead("Template", TemplateFile, TicketTemplate.Default);
            Log.WriteInfo("Initialization completed");
        }

        public async Task SetupTargetChat(string chatId)
        {
            Settings.TargetChatId = chatId;
            await SaveSettings(Settings);
        }

        public async Task SubscribeReport(string userId)
        {
            if (Settings.ReportSubscribers.Add(userId))
                await SaveSettings(Settings);
        }

        public async Task UnsubscibeReport(string userId)
        { 
            if (Settings.ReportSubscribers.Remove(userId))
                await SaveSettings(Settings);
        }

        private static async Task SaveSettings(SettingsSet settings)
        {
            Log.WriteInfo($"Saving settings to {SettingsFile}…");
            var saveResult = await FileExtension.TrySaveAsync(SettingsFile, settings);
            if (saveResult.IsSuccess)
                Log.WriteInfo($"Saving settings: success");
            else
            {
                Log.WriteInfo($"Saving settings: failed", saveResult.Exception);
                throw saveResult.Exception;
            }
        }

        private static async Task<T?> TryRead<T>(string valueName, string file, T defaultValue)
        {
            Log.WriteInfo($"Reading {valueName} from {file}…");
            var readResult = await FileExtension.TryReadAsync<T>(file);
            if (readResult.IsSuccess)
            {
                Log.WriteInfo($"Reading {valueName}: success");
                return readResult.Value;
            }
            else
            {
                Log.WriteInfo($"Reading {valueName}: failed", readResult.Exception);
                Log.WriteInfo($"Saving default {valueName} to {file}…");
                var saveResult = await FileExtension.TrySaveAsync(file, defaultValue);
                if (saveResult.IsSuccess)
                {
                    Log.WriteInfo($"Saving default {valueName}: success");
                    return defaultValue;
                }
                else
                {
                    Log.WriteInfo($"Saving default {valueName}: failed", saveResult.Exception);
                    throw saveResult.Exception;
                }
            }
        }
    }
}
