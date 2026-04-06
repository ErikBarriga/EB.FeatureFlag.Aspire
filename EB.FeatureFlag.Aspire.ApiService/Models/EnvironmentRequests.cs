namespace EB.FeatureFlag.Aspire.ApiService.Models;

public record CreateEnvironmentRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null);

public record UpdateEnvironmentRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null);
