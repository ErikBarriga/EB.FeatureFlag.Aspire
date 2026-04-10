using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Aspire.ApiService.Models;

public record SdkFeatureFlagResponse(
    string Product,
    string Environment,
    string Section,
    string Key,
    FeatureKeyType Type,
    object? Value);

public record SdkFeatureFlagValueResponse(
    FeatureKeyType Type,
    object? Value);
