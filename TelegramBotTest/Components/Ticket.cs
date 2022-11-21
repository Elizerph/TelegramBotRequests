namespace TelegramBotTest.Components
{
    public class Ticket
    {
        public int EditState { get; set; }
        public Dictionary<string, string> Fields { get; set; } = new();
    }
}
