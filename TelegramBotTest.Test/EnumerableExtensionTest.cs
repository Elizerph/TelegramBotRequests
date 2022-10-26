using TelegramBotTest.Utils;

namespace TelegramBotTest.Test
{
    [TestClass]
    public class EnumerableExtensionTest
    {
        [TestMethod]
        public void ByBatch()
        {
            var source = Enumerable.Range(0, 10);
            var result = source.ByBatch(5).ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(5, result[0].Count);
            Assert.AreEqual(5, result[1].Count);
        }

        [TestMethod]
        public void ByBatchCustomSize()
        {
            var source = Enumerable.Range(0, 10);
            var result = source.ByBatch(5, e => 2).ToArray();
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(2, result[0].Count);
            Assert.AreEqual(2, result[1].Count);
            Assert.AreEqual(2, result[2].Count);
            Assert.AreEqual(2, result[3].Count);
            Assert.AreEqual(2, result[4].Count);
        }
    }
}