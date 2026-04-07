namespace EB.FeatureFlag.Data.Repository.SQLite.Entities;

public class ExternalSourceConfigEntity
{
    public Guid Id { get; set; }
    public Guid FeatureKeyId { get; set; }
    public string Source { get; set; } = default!;
    public string Endpoint { get; set; } = default!;
    public string? AuthToken { get; set; }

    // Navigation property
    public FeatureKeyEntity FeatureKey { get; set; } = default!;

    // Headers as a separate child table
    public List<ExternalSourceHeaderEntity> Headers { get; set; } = [];
}