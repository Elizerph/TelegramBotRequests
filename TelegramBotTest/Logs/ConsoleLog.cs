namespace TelegramBotTest.Logs
{
    public class ConsoleLog : ILog
    {
        public Task WriteInfo(string text)
        {
            Console.WriteLine(text);
            return Task.CompletedTask;
        }
    }
}
