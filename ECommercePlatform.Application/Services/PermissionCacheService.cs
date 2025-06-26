using ECommercePlatform.Application.DTOs;
using ECommercePlatform.Application.Interfaces.IServices;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace ECommercePlatform.Application.Services
{
    public class PermissionCacheService(
            IMemoryCache cache,
            IPermissionService permissionService) : IPermissionCacheService
    {
        private readonly IMemoryCache _cache = cache;
        private readonly IPermissionService _permissionService = permissionService;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public async Task<List<UserPermissionDto>> GetUserPermissionsAsync(Guid userId)
        {
            var cacheKey = $"permissions_{userId}";

            if (!_cache.TryGetValue(cacheKey, out List<UserPermissionDto> permissions))
            {
                permissions = await _permissionService.GetUserPermissionsAsync(userId);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_cacheExpiration)
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set(cacheKey, permissions, cacheOptions);
            }

            return permissions;
        }

        public async Task InvalidateUserPermissionsAsync(Guid userId)
        {
            var cacheKey = $"permissions_{userId}";
            _cache.Remove(cacheKey);
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string module, string permission)
        {
            var permissions = await GetUserPermissionsAsync(userId);
            var modulePermission = permissions?.FirstOrDefault(p =>
                p.ModuleName?.Equals(module, StringComparison.OrdinalIgnoreCase) == true);

            if (modulePermission == null) return false;

            return permission.ToLower() switch
            {
                "view" => modulePermission.CanView,
                "addedit" => modulePermission.CanAddEdit,
                "delete" => modulePermission.CanDelete,
                _ => false
            };
        }

        public async Task InvalidateAllPermissionsAsync()
        {
            // Clear all cache entries by iterating over the cache keys
            var cacheKeys = AllCacheKeys;
            foreach (var key in cacheKeys)
            {
                _cache.Remove(key);
            }
        }

        private IEnumerable<object> AllCacheKeys
        {
            get
            {
                // Use reflection to access private cache entries
                var cacheEntriesField = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                var entriesCollection = cacheEntriesField?.GetValue(_cache) as dynamic;
                var cacheKeys = new List<object>();

                if (entriesCollection != null)
                {
                    foreach (var cacheItem in entriesCollection)
                    {
                        var keyProperty = cacheItem.GetType().GetProperty("Key");
                        var key = keyProperty?.GetValue(cacheItem);
                        if (key != null)
                        {
                            cacheKeys.Add(key);
                        }
                    }
                }

                return cacheKeys;
            }
        }
    }
}
