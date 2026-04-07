namespace EB.FeatureFlag.Data.Repository.SQLite.Entities;

public class ExternalSourceHeaderEntity
{
    public Guid Id { get; set; }
    public Guid ConfigId { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;

    // Navigation property
    public ExternalSourceConfigEntity Config { get; set; } = default!;
}