using System.Diagnostics.CodeAnalysis;

namespace TelegramBotTest
{
    public class BotCommandComparer : IEqualityComparer<string>
    {
        private readonly string _botMoniker;

        public BotCommandComparer(string botMoniker)
        {
            _botMoniker = botMoniker;
        }

        public bool Equals(string? x, string? y)
        {
            return string.Equals(EscapeMoniker(x), EscapeMoniker(y));
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return EscapeMoniker(obj).GetHashCode();
        }

        private string EscapeMoniker(string text)
        {
            return text.Replace(_botMoniker, string.Empty);
        }

        public static IEqualityComparer<string> FromMoniker(string moniker)
        { 
            return new BotCommandComparer(moniker);
        }
    }
}
