namespace EB.FeatureFlag.Data.ICache;

/// <summary>
/// Provides a standardized contract for caching operations, supporting both in-memory and distributed cache.
/// Allows specifying an absolute expiration time for each cached entry or batch.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached item by key.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores an item in the cache with an optional absolute expiration time.
    /// </summary>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? absoluteExpiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores multiple items in the cache with a single absolute expiration time for all.
    /// </summary>
    Task SetManyAsync<T>(
        IDictionary<string, T> items,
        TimeSpan? absoluteExpiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from the cache by key.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}