namespace EB.FeatureFlag.Data.Repository.SQLite.Entities;

public class FeatureFlagDetailEntity
{
    public Guid Id { get; set; }
    public Guid FeatureFlagId { get; set; }
    public Guid EnvironmentId { get; set; }
    public Guid ProductId { get; set; }
    public object? Value { get; set; }

    public ExternalSourceConfigEntity? ExternalConfig { get; set; }
}
