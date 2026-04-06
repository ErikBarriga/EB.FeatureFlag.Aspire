namespace EB.FeatureFlag.Aspire.ApiService.Models;

public record CreateSectionRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null);

public record UpdateSectionRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null);
