using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //if (isSuccess && error != null)
            //    throw new InvalidOperationException();
            //if (!isSuccess && value != null)
            //    throw new InvalidOperationException();
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static AppResult<T> Success(T value) => 
            new AppResult<T>(true, value, string.Empty);

        public static AppResult<T> Failure(string error) =>
            new AppResult<T>(false, default!, error);

    }

    public class AppResult
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        protected AppResult(bool isSuccess, string error)
        {
            //if (isSuccess && error != null)
            //    throw new InvalidOperationException();
            //if (!isSuccess && error == null)
            //    throw new InvalidOperationException();
            IsSuccess = isSuccess;
            Error = error;
        }

        public static AppResult Success() =>
            new AppResult(true, string.Empty);

        public static AppResult Failure(string error) =>
            new AppResult(false, error);
    }
}
