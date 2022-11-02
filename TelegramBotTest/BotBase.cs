using TelegramBotTest.Utils;

namespace TelegramBotTest
{
    public class BotBase
    {
        public static Stream? GetFile(BotFile file)
        {
            if (File.Exists(file.Name))
                return File.OpenRead(file.Name);
            else
                return null;
        }

        protected static async Task<T?> TryRead<T>(string valueName, string file, Func<string, Task<TryAsyncResult<T>>> read, Func<string, T, Task<TryAsyncResult>> save, T defaultValue)
        {
            Log.WriteInfo($"Reading {valueName} from {file}…");
            var readResult = await read(file);
            if (readResult.IsSuccess)
            {
                Log.WriteInfo($"Reading {valueName}: success");
                return readResult.Value;
            }
            else
            {
                Log.WriteInfo($"Reading {valueName}: failed", readResult.Exception);
                Log.WriteInfo($"Saving default {valueName} to {file}…");
                var saveResult = await save(file, defaultValue);
                if (saveResult.IsSuccess)
                {
                    Log.WriteInfo($"Saving default {valueName}: success");
                    return defaultValue;
                }
                else
                {
                    Log.WriteInfo($"Saving default {valueName}: failed", saveResult.Exception);
                    throw saveResult.Exception;
                }
            }
        }
    }
}
