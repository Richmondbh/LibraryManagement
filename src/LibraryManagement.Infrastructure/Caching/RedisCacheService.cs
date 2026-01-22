using LibraryManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Caching
{
    public class RedisCacheService: ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger) 
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedValue = await _cache.GetStringAsync(key, cancellationToken);

                if (string.IsNullOrEmpty(cachedValue))
                    return default;

                return JsonSerializer.Deserialize<T>(cachedValue, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
                };

                var serializedValue = JsonSerializer.Serialize(value, JsonOptions);
                await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);

                _logger.LogInformation("Cached value for key: {Key} with expiration: {Expiration}", key, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
                _logger.LogInformation("Removed cache for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            // Nb requires Redis server directly for SCAN command
            // For now, log a warning - we'll handle specific key removal
            _logger.LogWarning("RemoveByPrefixAsync is not fully implemented. Consider using specific keys.");
        }
    }
}
