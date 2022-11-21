namespace TelegramBotTest.Components
{
    public interface IRepository<TKey, TValue>
    {
        Task Save(TKey key, TValue value);
        Task<TValue?> Get(TKey key);
        IAsyncEnumerable<TValue> List();
        Task Clear();
    }
}
