namespace EB.FeatureFlag.Data;

public class FeatureFlagDataOptions
{
    public FeatureFlagRepositoryType RepositoryType { get; set; } = FeatureFlagRepositoryType.Cosmos;
    public string RepositoryConnectionString { get; set; } = string.Empty;
    public FeatureFlagCacheType CacheType { get; set; } = FeatureFlagCacheType.None;
    public string CacheConnectionString { get; set; } = string.Empty;
}