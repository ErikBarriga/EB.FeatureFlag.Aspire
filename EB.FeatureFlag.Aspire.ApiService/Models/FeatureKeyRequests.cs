using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Aspire.ApiService.Models;

public record CreateFeatureKeyRequest(
    string Name,
    FeatureKeyType Type,
    object? Value,
    string? Description = null,
    List<string>? Tags = null,
    string? ValidationRegex = null);

public record UpdateFeatureKeyRequest(
    string Name,
    FeatureKeyType Type,
    object? Value,
    string? Description = null,
    List<string>? Tags = null,
    string? ValidationRegex = null);
