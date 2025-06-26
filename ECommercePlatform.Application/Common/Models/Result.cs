namespace ECommercePlatform.Application.Common.Models
{
    public class AppResult<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public string Error { get; }

        private AppResult(bool isSuccess, T value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static AppResult<T> Success(T value) =>
            new(true, value, string.Empty);

        public static AppResult<T> Failure(string error) =>
            new(false, default!, error);
    }

    public class AppResult
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        protected AppResult(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static AppResult Success() =>
            new(true, string.Empty);

        public static AppResult Failure(string error) =>
            new(false, error);
    }
}
