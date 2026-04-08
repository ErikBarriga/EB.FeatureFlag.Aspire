using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Aspire.ApiService.Models;

public record CreateFeatureFlagRequest(
    string Name,
    FeatureKeyType Type,
    string? Description = null,
    List<string>? Tags = null,
    string? ValidationRegex = null);

public record UpdateFeatureFlagRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null,
    string? ValidationRegex = null);

public record UpdateFeatureFlagDetailRequest(
    object? Value,
    ExternalSourceConfigDto? ExternalConfig = null);
