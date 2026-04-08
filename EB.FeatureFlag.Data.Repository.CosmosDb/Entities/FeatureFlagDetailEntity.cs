using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Entities;

public class FeatureFlagDetailEntity
{
    public Guid Id { get; set; }
    public Guid FeatureFlagId { get; set; }
    public Guid EnvironmentId { get; set; }
    public Guid ProductId { get; set; }
    public object? Value { get; set; }
    public ExternalSourceConfigDto? ExternalConfig { get; set; }
}
