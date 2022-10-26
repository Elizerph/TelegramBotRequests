namespace TelegramBotTest
{
    public class BotResponseMessage
    {
        public long ChatId { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Buttons { get; set; }
        public BotFile? File { get; set; }
    }
}
