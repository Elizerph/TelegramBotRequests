using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBotTest.Logs;
using TelegramBotTest.Utils;

namespace TelegramBotTest
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly Bot _bot;

        public UpdateHandler(Bot bot)
        {
            _bot = bot;
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return Log.WriteInfo(exception.GetFullInfo());
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _bot.HandleRequest(GetRequest(update));
                var feedback = await ExecuteResponse(response, botClient);
                await _bot.HandleFeedback(feedback);
            }
            catch (Exception e)
            {
                await Log.WriteInfo(e.GetFullInfo());
                throw;
            }
        }

        private static BotUser GetUser(User user)
        {
            return new BotUser
            {
                Id = user.Id,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        private static BotChat GetChat(Chat chat)
        {
            return new BotChat
            { 
                Title = chat.Title,
                Id = chat.Id,
                IsPrivate = chat.Type == ChatType.Private
            };
        }

        private static BotRequest GetRequest(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    var message = update.Message;
                    return new BotRequest
                    {
                        User = GetUser(message.From),
                        Chat = GetChat(message.Chat),
                        MessageId = message.MessageId,
                        Text = message.Text,
                        File = GetFile(message.Document)
                    };
                case UpdateType.CallbackQuery:
                    var query = update.CallbackQuery;
                    return new BotRequest
                    {
                        User = GetUser(query.From),
                        Chat = GetChat(query.Message.Chat),
                        MessageId = query.Message.MessageId,
                        Text = query.Message.Text,
                        Button = query.Data
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(update.Type));
            }
        }

        private static BotFile GetFile(Document? document)
        {
            if (document == null)
                return null;

            return new BotFile
            { 
                Id = document.FileId,
                Name = document.FileName
            };
        }

        private static BotFeedbackMessage GetFeedbackMessage(Message message)
        {
            return new BotFeedbackMessage
            { 
                Chat = GetChat(message.Chat),
                Id = message.MessageId,
                User = GetUser(message.From),
                Text = message.Text
            };
        }

        private static async Task<BotFeedback> ExecuteResponse(BotResponse response, ITelegramBotClient client)
        {
            var postMessagesFeedback = new List<BotFeedbackMessage>();
            if (response.PostMessages != null)
                foreach (var postMessage in response.PostMessages)
                {
                    var buttons = postMessage.Buttons?.Select(p => InlineKeyboardButton.WithCallbackData(p.Value.InsertEmo(), p.Key)).ToArray() ?? Array.Empty<InlineKeyboardButton>();
                    Message message;
                    if (postMessage.File == null)
                        message = await client.SendTextMessageAsync(postMessage.ChatId, postMessage.Text.InsertEmo(), replyMarkup: new InlineKeyboardMarkup(buttons));
                    else
                        message = await client.SendDocumentAsync(postMessage.ChatId, GetDocument(postMessage.File), caption: postMessage.Text.InsertEmo(), replyMarkup: new InlineKeyboardMarkup(buttons));
                    postMessagesFeedback.Add(GetFeedbackMessage(message));
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            var editMessagesFeedback = new List<BotFeedbackMessage>();
            if (response.EditMessages != null)
                foreach (var editMessage in response.EditMessages)
                {
                    var buttons = editMessage.Buttons?.Select(p => InlineKeyboardButton.WithCallbackData(p.Value.InsertEmo(), p.Key)).ToArray() ?? Array.Empty<InlineKeyboardButton>();
                    var message = await client.EditMessageTextAsync(editMessage.ChatId, editMessage.MessageId, editMessage.Text.InsertEmo(), replyMarkup: new InlineKeyboardMarkup(buttons));
                    editMessagesFeedback.Add(GetFeedbackMessage(message));
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            var savedFiles = new List<string>();
            if (response.FilesToSave != null)
                foreach (var file in response.FilesToSave)
                {
                    var filePath = await client.GetFileAsync(file.Id);
                    using var fileStream = new FileStream(file.Name, FileMode.OpenOrCreate);
                    await client.DownloadFileAsync(filePath.FilePath, fileStream);
                    savedFiles.Add(file.Name);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            return new BotFeedback
            { 
                PostMessages = postMessagesFeedback,
                EditMessages = editMessagesFeedback,
                SavedFiles = savedFiles
            };
        }

        private static InputOnlineFile GetDocument(BotFile file)
        {
            var contentStream = BotBase.GetFile(file);
            if (contentStream == null)
                throw new InvalidOperationException($"File {file.Name} does not exists");
            else
                return new InputOnlineFile(contentStream, file.Name);
        }
    }
}
