namespace TelegramBotTest
{
    public class BotCommand
    {
        public string Moniker { get; set; }
        public string Description { get; set; }
        public Func<BotRequest, Task<BotResponse>> Method { get; set; }
    }
}
