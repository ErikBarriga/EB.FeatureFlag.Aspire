using EB.FeatureFlag.Data.ICache;
using System.Collections.Concurrent;

namespace EB.FeatureFlag.Data.Cache.InMemory;

/// <summary>
/// Simple thread-safe in-memory cache implementation of ICacheService.
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private class CacheEntry
    {
        public object Value { get; set; } = default!;
        public DateTimeOffset? Expiration { get; set; }
    }

    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiration == null || entry.Expiration > DateTimeOffset.UtcNow)
            {
                return Task.FromResult((T?)entry.Value);
            }
            _cache.TryRemove(key, out _);
        }
        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
    {
        var expiration = absoluteExpiration.HasValue ? DateTimeOffset.UtcNow.Add(absoluteExpiration.Value) : (DateTimeOffset?)null;
        _cache[key] = new CacheEntry { Value = value!, Expiration = expiration };
        return Task.CompletedTask;
    }

    public Task SetManyAsync<T>(IDictionary<string, T> items, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
    {
        var expiration = absoluteExpiration.HasValue ? DateTimeOffset.UtcNow.Add(absoluteExpiration.Value) : (DateTimeOffset?)null;
        foreach (var kvp in items)
        {
            _cache[kvp.Key] = new CacheEntry { Value = kvp.Value!, Expiration = expiration };
        }
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}