using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
            Console.WriteLine(string.Join(Environment.NewLine, new[] { exception.GetType().ToString(), exception.Message, exception.StackTrace }));
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var response = await _bot.Handle(GetRequest(update));
            await ExecuteResponse(response, botClient);
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
                Id = chat.Id
            };
        }

        private static BotRequest GetRequest(Update update)
        {
            var message = update.Message;
            switch (update.Type)
            {
                case UpdateType.Message:
                    return new BotRequest
                    { 
                        User = GetUser(message.From),
                        Chat = GetChat(message.Chat),
                        MessageId = message.MessageId,
                        Text = message.Text
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

        private static async Task ExecuteResponse(BotResponse response, ITelegramBotClient client)
        {
            if (response.PostMessages != null)
                foreach (var postMessage in response.PostMessages)
                {
                    var buttons = postMessage.Buttons?.Select(p => InlineKeyboardButton.WithCallbackData(p.Value, p.Key)).ToArray() ?? Array.Empty<InlineKeyboardButton>();
                    await client.SendTextMessageAsync(postMessage.ChatId, postMessage.Text, replyMarkup: new InlineKeyboardMarkup(buttons));
                }
            if (response.EditMessages != null)
                foreach (var editMessage in response.EditMessages)
                {
                    var buttons = editMessage.Buttons?.Select(p => InlineKeyboardButton.WithCallbackData(p.Value, p.Key)).ToArray() ?? Array.Empty<InlineKeyboardButton>();
                    await client.EditMessageTextAsync(editMessage.ChatId, editMessage.MessageId, editMessage.Text, replyMarkup: new InlineKeyboardMarkup(buttons));
                }
        }
    }
}
