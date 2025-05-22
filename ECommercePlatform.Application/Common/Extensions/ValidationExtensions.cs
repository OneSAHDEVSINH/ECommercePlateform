using ECommercePlatform.Application.Common.Models;

namespace ECommercePlatform.Application.Common.Extensions
{
    public static class ValidationExtensions
    {
        public static async Task<AppResult> EnsureUniqueAsync(
            this Task<bool> uniquenessCheckTask,
            string errorMessage)
        {
            var isUnique = await uniquenessCheckTask;
            return isUnique
                ? AppResult.Success()
                : AppResult.Failure(errorMessage);
        }

        public static async Task<AppResult<T>> EnsureUniqueAsync<T>(
            this Task<bool> uniquenessCheckTask,
            Func<T> onSuccess,
            string errorMessage)
        {
            var isUnique = await uniquenessCheckTask;
            return isUnique
                ? AppResult<T>.Success(onSuccess())
                : AppResult<T>.Failure(errorMessage);
        }
    }
}
