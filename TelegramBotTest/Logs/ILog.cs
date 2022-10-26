namespace TelegramBotTest.Logs
{
    public interface ILog
    {
        public Task WriteInfo(string text);
    }
}
