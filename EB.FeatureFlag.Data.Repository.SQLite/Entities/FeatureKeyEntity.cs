using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Repository.SQLite.Entities;

public class FeatureKeyEntity
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public Guid EnvironmentId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public FeatureKeyType Type { get; set; }
    public object? Value { get; set; }
    public string? ValidationRegex { get; set; }

    // Navigation property to child entity
    public ExternalSourceConfigEntity? ExternalConfig { get; set; }
}
