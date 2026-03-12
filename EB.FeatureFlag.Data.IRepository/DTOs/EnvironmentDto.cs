namespace EB.FeatureFlag.Data.IRepository.DTOs;

public class EnvironmentDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public string PrimaryAccessKey { get; set; } = default!;
    public string SecondaryAccessKey { get; set; } = default!;
}