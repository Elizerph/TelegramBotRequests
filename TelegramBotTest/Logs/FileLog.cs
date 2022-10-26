namespace TelegramBotTest.Logs
{
    public class FileLog : ILog
    {
        private readonly string _fileName;

        public FileLog(string fileName)
        {
            _fileName = fileName;
        }

        public Task WriteInfo(string text)
        {
            return File.AppendAllLinesAsync(_fileName, new[] { text });
        }
    }
}
