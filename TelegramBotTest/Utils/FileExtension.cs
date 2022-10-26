using Newtonsoft.Json;

namespace TelegramBotTest.Utils
{
    public static class FileExtension
    {
        public static async Task<TryAsyncResult<T>> TryReadAsync<T>(string file)
        {
            try
            {
                var text = await File.ReadAllTextAsync(file);
                var result = JsonConvert.DeserializeObject<T>(text);
                return TryAsyncResult.FromResult(result);
            }
            catch (Exception e)
            {
                return TryAsyncResult.FromException<T>(e);
            }
        }

        public static async Task<TryAsyncResult<long>> TryReadLongAsync(string file)
        {
            try
            {
                var text = await File.ReadAllTextAsync(file);
                var result = long.Parse(text);
                return TryAsyncResult.FromResult(result);
            }
            catch (Exception e)
            {
                return TryAsyncResult.FromException<long>(e);
            }
        }

        public static async Task<TryAsyncResult> TrySaveAsync<T>(string file, T value)
        {
            try
            {
                var text = JsonConvert.SerializeObject(value);
                await File.WriteAllTextAsync(file, text);
                return TryAsyncResult.Success;
            }
            catch (Exception e)
            {
                return TryAsyncResult.FromException(e);
            }
        }

        public static async Task<TryAsyncResult> TrySaveLongAsync(string file, long value)
        {
            try
            {
                await File.WriteAllTextAsync(file, value.ToString());
                return TryAsyncResult.Success;
            }
            catch (Exception e)
            {
                return TryAsyncResult.FromException(e);
            }
        }

    }
}
