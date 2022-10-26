namespace TelegramBotTest.Logs
{
    public class MultiLog : ILog
    {
        private readonly IReadOnlyCollection<ILog> _logs;

        public MultiLog(IReadOnlyCollection<ILog> logs)
        {
            _logs = logs;
        }

        public Task WriteInfo(string text)
        {
            return Task.WhenAll(_logs.Select(e => e.WriteInfo(text)));
        }
    }
}
