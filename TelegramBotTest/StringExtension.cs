namespace TelegramBotTest
{
    public static class StringExtension
    {
        private static readonly char[] _newLineChars = new[] { '\n', '\r' }; 

        public static string[] SplitLines(this string text)
        {
            return text.Split(_newLineChars, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
