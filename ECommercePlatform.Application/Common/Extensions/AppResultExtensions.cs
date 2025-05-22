using ECommercePlatform.Application.Common.Models;

namespace ECommercePlatform.Application.Common.Extensions
{
    public static class AppResultExtensions
    {
        public static async Task<AppResult<TOut>> BindAsync<TIn, TOut>(
            this Task<AppResult<TIn>> task,
            Func<TIn, Task<AppResult<TOut>>> func)
        {
            var result = await task;
            if (result.IsFailure)
            {
                return AppResult<TOut>.Failure(result.Error);
            }

            return await func(result.Value);
        }
    }
}
