using ElizerBot.Adapter.Triggers;

namespace TelegramBotTest.Components
{
    internal class PrivateChatMessageTrigger : MessageTrigger<Context>
    {
        public PrivateChatMessageTrigger(Func<Context, MessageTriggerArgument, Task> action) 
            : base(action)
        {
        }

        public override bool Validate(MessageTriggerArgument arg)
        {
            return arg.Message.Chat.IsPrivate && !string.IsNullOrEmpty(arg.Message.Text);
        }
    }
}
