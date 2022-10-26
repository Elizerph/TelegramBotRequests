namespace TelegramBotTest.Logs
{
    public static class Log
    {
        private readonly static ILog _instance = new LogDecorator(new MultiLog(new ILog[]
        {
            new ConsoleLog(),
            new FileLog("log.txt")
        }), text => $"{DateTime.Now}: {text}");

        public static Task WriteInfo(string text)
        {
            return _instance.WriteInfo(text);
        }
    }
}
