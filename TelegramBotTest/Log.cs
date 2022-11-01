using log4net;

namespace TelegramBotTest
{
    public static class Log
    {
        private readonly static ILog _instance = LogManager.GetLogger(typeof(Program));

        public static void WriteInfo(string text)
        {
            _instance.Info(text);
        }

        public static void WriteInfo(string text, Exception exception)
        {
            _instance.Error(text, exception);
        }
    }
}
