using ElizerBot.Adapter.Triggers;

namespace TelegramBotTest.Components
{
    public class PrivateChatMessageTrigger : MessageTrigger<Context>
    {
        public PrivateChatMessageTrigger(Func<Context, MessageTriggerArgument, Task> action)
            : base(action)
        {
        }

        public override Task<bool> Validate(MessageTriggerArgument arg)
        {
            return Task.FromResult(arg.Message.Chat.IsPrivate && !string.IsNullOrEmpty(arg.Message.Text));
        }
    }
}
