namespace TelegramBotTest.Components
{
    public class SettingsSet
    {
        public static SettingsSet Default { get; } = new SettingsSet 
        {
            ReportSubscribers = new HashSet<string>(),
            ReportTime = new DateTime(2022, 11, 21, 19, 0, 0),
            TargetChatId = "0"
        };

        public string TargetChatId { get; set; }
        public HashSet<string> ReportSubscribers { get; set; }
        public DateTime ReportTime { get; set; }
    }
}
