namespace EB.FeatureFlag.Data.IRepository.DTOs;

public class SectionDto
{
    public Guid Id { get; set; }
    public Guid EnvironmentId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}