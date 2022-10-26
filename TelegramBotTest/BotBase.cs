using TelegramBotTest.Logs;
using TelegramBotTest.Utils;

namespace TelegramBotTest
{
    public class BotBase
    {
        private readonly long _adminId;

        public BotBase(long adminId)
        {
            _adminId = adminId;
        }

        protected bool IsAdmin(long userId)
        { 
            return userId == _adminId;
        }

        protected bool IsAdmin(BotUser user)
        {
            return IsAdmin(user.Id);
        }

        protected bool IsAdminRequest(BotRequest request)
        {
            return request.Chat.IsPrivate && IsAdmin(request.User);
        }

        public static Stream? GetFile(BotFile file)
        {
            if (File.Exists(file.Name))
                return File.OpenRead(file.Name);
            else
                return null;
        }

        protected static async Task<T?> TryRead<T>(string valueName, string file, Func<string, Task<TryAsyncResult<T>>> read, Func<string, T, Task<TryAsyncResult>> save, T defaultValue)
        {
            await Log.WriteInfo($"Reading {valueName} from {file}…");
            var readResult = await read(file);
            if (readResult.IsSuccess)
            {
                await Log.WriteInfo($"Reading {valueName}: success");
                return readResult.Value;
            }
            else
            {
                await Log.WriteInfo($"Reading {valueName}: failed - {readResult.Exception.GetFullInfo()}");
                await Log.WriteInfo($"Saving default {valueName} to {file}…");
                var saveResult = await save(file, defaultValue);
                if (saveResult.IsSuccess)
                {
                    await Log.WriteInfo($"Saving default {valueName}: success");
                    return defaultValue;
                }
                else
                {
                    await Log.WriteInfo($"Saving default {valueName}: failed - {saveResult.Exception.GetFullInfo()}");
                    throw saveResult.Exception;
                }
            }
        }
    }
}
