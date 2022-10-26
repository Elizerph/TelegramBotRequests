namespace TelegramBotTest.Utils
{
    public static class ExceptionExtension
    {
        public static string GetFullInfo(this Exception exception)
        {
            return string.Join(Environment.NewLine, new[]
            {
                exception.GetType().ToString(),
                exception.Message,
                exception.StackTrace
            });
        }
    }
}
