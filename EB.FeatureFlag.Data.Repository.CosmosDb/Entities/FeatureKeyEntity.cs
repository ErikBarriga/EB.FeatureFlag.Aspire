using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Entities;

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
}