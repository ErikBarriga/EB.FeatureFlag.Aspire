using StackExchange.Redis;
using System.Text.Json;
using EB.FeatureFlag.Data.ICache;

namespace EB.FeatureFlag.Data.Cache.Redis;

/// <summary>
/// Distributed Redis cache implementation of ICacheService.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(string connectionString)
    {
        var muxer = ConnectionMultiplexer.Connect(connectionString);
        _db = muxer.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, absoluteExpiration, false);
    }

    public async Task SetManyAsync<T>(IDictionary<string, T> items, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();
        foreach (var kvp in items)
        {
            var json = JsonSerializer.Serialize(kvp.Value);
            tasks.Add(_db.StringSetAsync(kvp.Key, json, absoluteExpiration, false));
        }
        await Task.WhenAll(tasks);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _db.KeyDeleteAsync(key);
    }
}