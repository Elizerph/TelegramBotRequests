using ElizerBot.Adapter;

using TelegramBotTest.Utils;

namespace TelegramBotTest.Components
{
    public static class BotExtension
    {
        private readonly static Random _random = new();
        private readonly static string[] _emos = new[]
        {
            "<emo1F60A>",
            "<emo1F61B>",
            "<emo1F61C>",
            "<emo1F61D>",
            "<emo1F609>",
            "<emo1F643>",
            "<emo1F92A>"
        };

        public static Task<PostedMessageAdapter> SendRandomEmo(this IBotAdapter bot, ChatAdapter chat)
        {
            var message = new NewMessageAdapter(chat)
            {
                Text = _emos[_random.Next(0, _emos.Length)].InsertEmo()
            };
            return bot.SendMessage(message);
        }
    }
}
