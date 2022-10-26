namespace TelegramBotTest
{
    public class BotFeedback
    {
        public IReadOnlyCollection<BotFeedbackMessage> PostMessages { get; set; }
        public IReadOnlyCollection<BotFeedbackMessage> EditMessages { get; set; }
        public IReadOnlyCollection<string> SavedFiles { get; set; }
    }
}
