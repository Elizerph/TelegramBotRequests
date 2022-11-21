using System.Collections.Concurrent;

namespace TelegramBotTest.Components
{
    public class MemoryMessageRepository : IMessageRepository
    {
        private readonly ConcurrentDictionary<string, string> _messages = new();

        public Task Save(string key, string value)
        {
            _messages.AddOrUpdate(key, value, (k, v) => value);
            return Task.CompletedTask;
        }

        public Task<string?> Get(string key)
        {
            _messages.TryGetValue(key, out var value);
            return Task.FromResult(value);
        }

        public IAsyncEnumerable<string> List()
        {
            return _messages.Values.ToAsyncEnumerable();
        }

        public Task Clear()
        {
            _messages.Clear();
            return Task.CompletedTask;
        }
    }
}
