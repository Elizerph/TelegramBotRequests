namespace TelegramBotTest.Utils
{
    public class TryAsyncResult
    {
        public static TryAsyncResult Success => new (true, null);
        public bool IsSuccess { get; }
        public Exception? Exception { get; }
        public TryAsyncResult(bool isSuccess, Exception? exception)
        { 
            IsSuccess = isSuccess;
            Exception = exception;
        }

        public static TryAsyncResult FromException(Exception exception)
        {
            return new TryAsyncResult(false, exception);
        }

        public static TryAsyncResult<T> FromException<T>(Exception exception)
        {
            return new TryAsyncResult<T>(false, exception, default);
        }

        public static TryAsyncResult<T> FromResult<T>(T? result)
        {
            return new TryAsyncResult<T>(true, null, result);
        }
    }

    public class TryAsyncResult<T> : TryAsyncResult
    {
        public T? Value { get; }

        public TryAsyncResult(bool isSuccess, Exception? exception, T? value)
            : base(isSuccess, exception)
        {
            Value = value;
        }
    }
}
