using Newtonsoft.Json;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotTest
{
    public class UpdateHandler : IUpdateHandler
    {
        public readonly static string NewRequestCommand = "/setthischat";
        public readonly static string SetChatCommand = "/newrequest";

        private const string TargetChatIdFile = "ChatId.txt";
        private const string TemplateFile = "Template.json";

        private long _targetChatId;
        private RequestTemplate _template = new()
        {
            Title = "Новая заявка",
            AcceptButtonLabel = "Принять",
            AcceptedTitle = "Заявка принята <user>",
            RejectButtonLabel = "Отклонить",
            RejectedTitle = "Заявка отклонена <user>",
            CompletedMessage = "Заявка создана",
            FieldNames = new[] { "Тема", "Телефон", "Адрес", "Время" } 
        };
        private readonly Dictionary<long, Request> _requests = new();

        public async Task Init()
        {
            if (!System.IO.File.Exists(TargetChatIdFile))
                await System.IO.File.WriteAllTextAsync(TargetChatIdFile, "0");
            _targetChatId = long.Parse(await System.IO.File.ReadAllTextAsync(TargetChatIdFile));
            if (!System.IO.File.Exists(TemplateFile))
                await System.IO.File.WriteAllTextAsync(TemplateFile, JsonConvert.SerializeObject(_template));
            var templateText = await System.IO.File.ReadAllTextAsync(TemplateFile);
            _template = JsonConvert.DeserializeObject<RequestTemplate>(templateText);
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            switch (update.Type)
            {
                case UpdateType.Message:
                    var messageText = message.Text;
                    var userId = message.From.Id;
                    var chatId = message.Chat.Id;
                    if (string.Equals(messageText, SetChatCommand))
                    {
                        _targetChatId = chatId;
                        await System.IO.File.WriteAllTextAsync(TargetChatIdFile, _targetChatId.ToString(), cancellationToken);
                        Console.WriteLine($"Chat to post requests: {message.Chat.Title}");
                    }
                    else if (string.Equals(messageText, NewRequestCommand))
                    {
                        var newRequest = new Request();
                        _requests[userId] = newRequest;
                        await botClient.SendTextMessageAsync(chatId, $"{_template.FieldNames[0]}:", cancellationToken: cancellationToken);
                    } 
                    else if (_requests.TryGetValue(userId, out var request))
                    {
                        request.Fields[_template.FieldNames[request.EditState]] = messageText;
                        request.EditState++;
                        if (request.EditState < _template.FieldNames.Length)
                        {
                            await botClient.SendTextMessageAsync(chatId, $"{_template.FieldNames[request.EditState]}:", cancellationToken: cancellationToken);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, _template.CompletedMessage, cancellationToken: cancellationToken);

                            var response = new List<string>
                            {
                                _template.Title
                            };
                            response.AddRange(request.Fields.Select(p => $"{p.Key}: {p.Value}"));
                            var buttons = new[]
                            {
                                    InlineKeyboardButton.WithCallbackData(_template.AcceptButtonLabel),
                                    InlineKeyboardButton.WithCallbackData(_template.RejectButtonLabel)
                            };
                            var markup = new InlineKeyboardMarkup(buttons);
                            if (_targetChatId != 0L)
                                await botClient.SendTextMessageAsync(_targetChatId, string.Join(Environment.NewLine, response), replyMarkup: markup, cancellationToken: cancellationToken);
                            _requests.Remove(userId);
                        }
                    }
                    break;
                case UpdateType.CallbackQuery:
                    message = update.CallbackQuery.Message;
                    var data = update.CallbackQuery.Data;
                    var from = update.CallbackQuery.From;
                    var fromFullName = $"{from.FirstName} {from.LastName}".Trim();
                    var titleTemplate = "newState";
                    if (string.Equals(data, _template.AcceptButtonLabel))
                    {
                        titleTemplate = _template.AcceptedTitle;
                    }
                    else if (string.Equals(data, _template.RejectButtonLabel))
                    {
                        titleTemplate = _template.RejectedTitle;
                    }
                    var newTitle = titleTemplate.Replace("<user>", $"{from.Username} ({fromFullName})");
                    await botClient.EditMessageTextAsync(message.Chat.Id, message.MessageId, message.Text.Replace(_template.Title, newTitle), cancellationToken: cancellationToken);
                    break;
                default:
                    break;
            }
        }
    }
}
