namespace TelegramBotTest
{
    public class BotButton
    {
        public string Moniker { get; set; }
        public string Label { get; set; }
        public Func<BotRequest, string[], Task<BotResponse>> Method { get; set; }
    }
}
