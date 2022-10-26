namespace TelegramBotTest.Logs
{
    public class LogDecorator : ILog
    {
        private readonly ILog _log;
        private readonly Func<string, string> _decorator;

        public LogDecorator(ILog log, Func<string, string> decorator)
        {
            _log = log;
            _decorator = decorator;
        }

        public Task WriteInfo(string text)
        {
            return _log.WriteInfo(_decorator(text));
        }
    }
}
