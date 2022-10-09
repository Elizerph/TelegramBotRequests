namespace TelegramBotTest
{
    public class BotRequest
    {
        public BotChat Chat { get; set; }
        public BotUser User { get; set; }
        public string Button { get; set; }
        public int MessageId { get; set; }
        public string Text { get; set; }
    }
}
