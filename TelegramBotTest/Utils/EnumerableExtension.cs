namespace TelegramBotTest.Utils
{
    public static class EnumerableExtension
    {
        public static IEnumerable<IReadOnlyCollection<T>> ByBatch<T>(this IEnumerable<T> source, int batchSize, Func<T, int> getItemSize)
        {
            var batch = new List<T>();
            var currentBatchSize = 0;
            foreach (var item in source)
            {
                var itemSize = getItemSize(item);
                if (itemSize > batchSize)
                    throw new InvalidOperationException($"Item {item} size is over {batchSize} batch size");
                var nextBatchSize = currentBatchSize + itemSize;
                if (nextBatchSize > batchSize)
                {
                    yield return batch;
                    batch = new List<T>()
                    {
                        item
                    };
                    currentBatchSize = itemSize;
                }
                else
                {
                    batch.Add(item);
                    currentBatchSize = nextBatchSize;
                }
            }
            yield return batch;
        }

        public static IEnumerable<IReadOnlyCollection<T>> ByBatch<T>(this IEnumerable<T> source, int batchSize)
        {
            return source.ByBatch(batchSize, e => 1);
        }
    }
}
