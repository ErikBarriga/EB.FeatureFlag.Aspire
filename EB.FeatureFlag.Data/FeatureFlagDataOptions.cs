namespace EB.FeatureFlag.Data;

public class FeatureFlagDataOptions
{
    public FeatureFlagRepositoryType RepositoryType { get; set; } = FeatureFlagRepositoryType.Cosmos;
    public string RepositoryConnectionString { get; set; } = string.Empty;
    public FeatureFlagCacheType CacheType { get; set; } = FeatureFlagCacheType.None;
    public string CacheConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// TTL in seconds for SDK feature flag cache entries. Valid range: 10 seconds to 604800 seconds (7 days). Default: 300 (5 minutes).
    /// </summary>
    public int SdkCacheTtlSeconds { get; set; } = 300;
}