using Newtonsoft.Json;

using Telegram.Bot;

using File = System.IO.File;

namespace TelegramBotTest
{
    public class Bot
    {
        private Dictionary<string, Func<BotRequest, Task<BotResponse>>> _commands;
        private Dictionary<string, Func<BotRequest, string[], Task<BotResponse>>> _buttons;

        private const string TargetChatIdFile = "ChatId.txt";
        private const string TemplateFile = "Template.json";

        private long _targetChatId;
        private RequestTemplate _template = new()
        {
            Title = "Новая заявка",
            AcceptButtonLabel = "Принять",
            AcceptedTitle = "Заявка принята <user>",
            DoneTitle = "Выполнено <user>",
            DropTitle = "Не выполнено <user>",
            DoneButtonLabel = "Выполнено",
            DropButtonLabel = "Не выполнено",
            CompletedMessage = "Заявка создана",
            FieldNames = new[] { "Тема", "Телефон", "Адрес", "Время" }
        };
        private readonly Dictionary<long, Request> _requests = new();

        public async Task Init(ITelegramBotClient botClient)
        {
            if (!File.Exists(TargetChatIdFile))
                await File.WriteAllTextAsync(TargetChatIdFile, "0");
            _targetChatId = long.Parse(await File.ReadAllTextAsync(TargetChatIdFile));
            if (!File.Exists(TemplateFile))
                await File.WriteAllTextAsync(TemplateFile, JsonConvert.SerializeObject(_template));
            var templateText = await File.ReadAllTextAsync(TemplateFile);
            _template = JsonConvert.DeserializeObject<RequestTemplate>(templateText);

            var buttons = new[]
            {
                new BotButton { Moniker = "accept", Label = _template.AcceptButtonLabel, Method = ExecuteButtonAccept },
                new BotButton { Moniker = "done", Label = _template.DoneButtonLabel, Method = ExecuteButtonDone },
                new BotButton { Moniker = "drop", Label = _template.DropButtonLabel, Method = ExecuteButtonDrop }
            };

            _buttons = buttons.ToDictionary(e => e.Moniker, e => e.Method);

            var commands = new[]
            {
                new BotCommand { Moniker = "/newrequest", Description = "Новая заявка", Method = ExecuteNewRequest },
                new BotCommand { Moniker = "/setthischat", Description = "Задать этот чат для заявок", Method = ExecuteSetThisChat }
            };

            var botMe = await botClient.GetMeAsync();
            _commands = commands.ToDictionary(e => e.Moniker, e => e.Method, BotCommandComparer.FromMoniker($"@{botMe.Username}"));
            await botClient.DeleteMyCommandsAsync();
            await botClient.SetMyCommandsAsync(commands.Select(e => new Telegram.Bot.Types.BotCommand { Command = e.Moniker, Description = e.Description }));
        }

        private static string ReplaceWithUser(string text, BotUser user)
        {
            return text.Replace("<user>", $"@{user.UserName} ({$"{user.FirstName} {user.LastName}".Trim()})");
        }

        private static string ChangeTitle(string text, string newTitle)
        {
            var lines = text.SplitLines().ToList();
            lines.RemoveAt(0);
            lines.Insert(0, newTitle);
            return string.Join(Environment.NewLine, lines);
        }

        public async Task<BotResponse> Handle(BotRequest botRequest)
        {
            if (!string.IsNullOrEmpty(botRequest.Button))
            {
                var buttonData = botRequest.Button.Split('$');
                var action = buttonData[0];
                var parameters = buttonData.Skip(1).ToArray();
                if (_buttons.TryGetValue(action, out var buttonMethod))
                    return await buttonMethod(botRequest, parameters);
            }

            if (string.IsNullOrWhiteSpace(botRequest.Text))
                return BotResponse.Empty;

            if (_commands.TryGetValue(botRequest.Text, out var command))
                return await command(botRequest);

            if (_requests.TryGetValue(botRequest.User.Id, out var request))
            {
                request.Fields[_template.FieldNames[request.EditState]] = botRequest.Text;
                request.EditState++;
                if (request.EditState < _template.FieldNames.Length)
                    return BotResponse.FromPostMessage(botRequest.Chat.Id, $"{_template.FieldNames[request.EditState]}:");

                var postMessages = new List<BotResponseMessage>
                { 
                    new BotResponseMessage
                    { 
                        ChatId = botRequest.Chat.Id,
                        Text = _template.CompletedMessage
                    }
                };

                if (_targetChatId != 0L)
                {
                    var requestTextLines = new List<string>
                    { 
                        _template.Title
                    };
                    requestTextLines.AddRange(request.Fields.Select(p => $"{p.Key}: {p.Value}"));
                    postMessages.Add(new BotResponseMessage 
                    { 
                        ChatId = _targetChatId,
                        Text = string.Join(Environment.NewLine, requestTextLines),
                        Buttons = new Dictionary<string, string>
                        {
                            { "accept", _template.AcceptButtonLabel }
                        }
                    });
                }
                _requests.Remove(botRequest.User.Id);
                return new BotResponse
                { 
                    PostMessages = postMessages
                };
            }

            return BotResponse.Empty;
        }

        private async Task<BotResponse> ExecuteNewRequest(BotRequest request)
        {
            var newRequest = new Request();
            _requests[request.User.Id] = newRequest;
            return BotResponse.FromPostMessage(request.Chat.Id, $"{_template.FieldNames[0]}:");
        }

        private async Task<BotResponse> ExecuteSetThisChat(BotRequest request)
        {
            _targetChatId = request.Chat.Id;
            await File.WriteAllTextAsync(TargetChatIdFile, _targetChatId.ToString());
            Console.WriteLine($"Chat to post requests: {request.Chat.Title}");
            return BotResponse.Empty;
        }

        private async Task<BotResponse> ExecuteButtonAccept(BotRequest botRequest, string[] parameters)
        {
            return new BotResponse
            {
                EditMessages = new[]
                {
                    new BotReponseEditMessage
                    {
                        ChatId = botRequest.Chat.Id,
                        MessageId = botRequest.MessageId,
                        Text = ChangeTitle(botRequest.Text, ReplaceWithUser(_template.AcceptedTitle, botRequest.User)),
                        Buttons = new Dictionary<string, string>
                        {
                            { $"done${botRequest.User.Id}", _template.DoneButtonLabel },
                            { $"drop${botRequest.User.Id}", _template.DropButtonLabel }
                        }
                    }
                }
            };
        }

        private Task<BotResponse> ExecuteButtonDone(BotRequest botRequest, string[] parameters)
        {
            if (parameters.Length >= 1 && long.TryParse(parameters[0], out var userId) && userId == botRequest.User.Id)
            {
                var newText = ReplaceWithUser(_template.DoneTitle, botRequest.User);
                var response = GetChangeMessageTitleResponse(botRequest.Chat.Id, botRequest.MessageId, botRequest.Text, newText);
                return Task.FromResult(response);
            }
            return Task.FromResult(BotResponse.Empty);
        }

        private Task<BotResponse> ExecuteButtonDrop(BotRequest botRequest, string[] parameters)
        {
            if (parameters.Length >= 1 && long.TryParse(parameters[0], out var userId) && userId == botRequest.User.Id)
            {
                var newText = ReplaceWithUser(_template.DropTitle, botRequest.User);
                var response = GetChangeMessageTitleResponse(botRequest.Chat.Id, botRequest.MessageId, botRequest.Text, newText);
                return Task.FromResult(response);
            }
            return Task.FromResult(BotResponse.Empty);
        }

        private static BotResponse GetChangeMessageTitleResponse(long chatId, int messageId, string text, string newTitle)
        {
            return new BotResponse
            {
                EditMessages = new[]
                {
                    new BotReponseEditMessage
                    {
                        ChatId = chatId,
                        MessageId = messageId,
                        Text = ChangeTitle(text, newTitle)
                    }
                }
            };
        }
    }
}
