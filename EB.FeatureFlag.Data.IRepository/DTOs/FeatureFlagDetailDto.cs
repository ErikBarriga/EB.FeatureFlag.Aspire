namespace EB.FeatureFlag.Data.IRepository.DTOs;

public class FeatureFlagDetailDto
{
    public Guid Id { get; set; }
    public Guid FeatureFlagId { get; set; }
    public Guid EnvironmentId { get; set; }
    public Guid ProductId { get; set; }
    public object? Value { get; set; }
    public ExternalSourceConfigDto? ExternalConfig { get; set; }
}
