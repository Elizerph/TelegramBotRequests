using TelegramBotTest.Utils;

namespace TelegramBotTest.Components
{
    internal class Context
    {
        private const string TargetChatIdFile = "Data/ChatId.txt";
        private const string TemplateFile = "Data/Template.json";
        private static readonly TicketTemplate _defaultTemplate = new()
        {
            NewTicketTitle = "<emo1F4C4> Новая заявка",
            AcceptedTicketTitle = "<emo1F552> Заявка в работе",
            DoneTicketTitle = "<emo2705> Заявка выполнена",
            DropTicketTitle = "<emo274C> Заявка отклонена",
            CompletedMessage = "Заявка создана",
            AcceptButtonLabel = "<emo1F446> Принять",
            DoneButtonLabel = "<emo2705> Выполнено",
            DropButtonLabel = "<emo274C> Отклонено",
            NewTicketHistory = "<time>: <user> создано",
            AcceptedTicketHistory = "<time>: <user> принято",
            DoneTicketHistory = "<time>: <user> выполнено",
            DropTicketHistory = "<time>: <user> отклонено",
            FieldNames = new[] 
            { 
                "Тема", 
                "Телефон", 
                "Адрес", 
                "Время" 
            }
        };

        public string TargetChatId { get; private set; }
        public Dictionary<string, Ticket> EditingTickets { get; } = new();
        public TicketTemplate Template { get; set; }

        internal async Task Init()
        {
            Log.WriteInfo("Initialization…");
            TargetChatId = await TryReadChatId();
            Template = await TryReadTemplate();
            Log.WriteInfo("Initialization completed");
        }

        internal async Task SetupTargetChat(string id)
        {
            TargetChatId = id;
            Log.WriteInfo($"Chat to post requests: {id}");
            Log.WriteInfo($"Saving TargetChatId to {TargetChatIdFile}…");
            var saveResult = await FileExtension.TrySaveAsync(TargetChatIdFile, TargetChatId);
            if (saveResult.IsSuccess)
                Log.WriteInfo($"Saving TargetChatId: success");
            else
            {
                Log.WriteInfo($"Saving TargetChatId: failed", saveResult.Exception);
                throw saveResult.Exception;
            }
        }

        protected static async Task<T?> TryRead<T>(string valueName, string file, Func<string, Task<TryAsyncResult<T>>> read, Func<string, T, Task<TryAsyncResult>> save, T defaultValue)
        {
            Log.WriteInfo($"Reading {valueName} from {file}…");
            var readResult = await read(file);
            if (readResult.IsSuccess)
            {
                Log.WriteInfo($"Reading {valueName}: success");
                return readResult.Value;
            }
            else
            {
                Log.WriteInfo($"Reading {valueName}: failed", readResult.Exception);
                Log.WriteInfo($"Saving default {valueName} to {file}…");
                var saveResult = await save(file, defaultValue);
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

        private static async Task<string?> TryReadChatId()
        {
            return await TryRead("TargetChatId", TargetChatIdFile, FileExtension.TryReadAsync, FileExtension.TrySaveAsync, string.Empty);
        }

        private static Task<TicketTemplate?> TryReadTemplate()
        {
            return TryRead("Template", TemplateFile, FileExtension.TryReadAsync<TicketTemplate>, FileExtension.TrySaveAsync, _defaultTemplate);
        }
    }
}
