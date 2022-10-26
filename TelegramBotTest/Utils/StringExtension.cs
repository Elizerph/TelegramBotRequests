using System.Globalization;
using System.Text.RegularExpressions;

namespace TelegramBotTest.Utils
{
    public static class StringExtension
    {
        private static readonly char[] _newLineChars = new[] { '\n', '\r' };

        public static string[] SplitLines(this string text)
        {
            return text.Split(_newLineChars, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string InsertEmo(this string text)
        {
            var r = new Regex("<emo(?<value>.*?)>");
            foreach (var match in r.Matches(text).Cast<Match>())
            {
                var code = match.Groups["value"].Value;
                text = text.Replace(match.Value, char.ConvertFromUtf32(int.Parse(code, NumberStyles.HexNumber)));
            }
            return text;
        }
    }
}
