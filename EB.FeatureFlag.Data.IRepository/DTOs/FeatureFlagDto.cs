using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.IRepository.DTOs;

public class FeatureFlagDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid SectionId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public FeatureKeyType Type { get; set; }
    public string? ValidationRegex { get; set; }
}
