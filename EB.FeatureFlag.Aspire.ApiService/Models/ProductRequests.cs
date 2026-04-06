namespace EB.FeatureFlag.Aspire.ApiService.Models;

public record CreateProductRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null);

public record UpdateProductRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null);
