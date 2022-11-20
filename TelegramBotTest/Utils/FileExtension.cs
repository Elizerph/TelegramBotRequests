using Newtonsoft.Json;

namespace TelegramBotTest.Utils
{
    public static class FileExtension
    {
        public static async Task<TryAsyncResult<string>> TryReadAsync(string file)
        {
            try
            {
                var text = await File.ReadAllTextAsync(file);
                return TryAsyncResult.FromResult(text);
            }
            catch (Exception e)
            {
                return TryAsyncResult.FromException<string>(e);
            }
        }

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

        public static async Task<TryAsyncResult> TrySaveAsync(string file, string value)
        {
            try
            {
                await File.WriteAllTextAsync(file, value);
                return TryAsyncResult.Success;
            }
            catch (Exception e)
            {
                return TryAsyncResult.FromException(e);
            }
        }

        public static Task<TryAsyncResult> TrySaveAsync<T>(string file, T value)
        {
            return TrySaveAsync(file, JsonConvert.SerializeObject(value));
        }
    }
}
