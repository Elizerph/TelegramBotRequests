namespace TelegramBotTest
{
    public class BotResponse
    {
        public static BotResponse Empty => new();
        public IReadOnlyCollection<BotResponseMessage> PostMessages { get; set; }
        public IReadOnlyCollection<BotReponseEditMessage> EditMessages { get; set; }
        public IReadOnlyCollection<BotFile> FilesToSave { get; set; }
        public static BotResponse FromPostMessage(long chatId, string text)
        {
            return new BotResponse
            {
                PostMessages = new[] 
                { 
                    new BotResponseMessage
                    {
                        ChatId = chatId,
                        Text = text
                    }
                }
            };
        }
    }
}
