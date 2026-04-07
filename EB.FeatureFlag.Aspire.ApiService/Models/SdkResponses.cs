using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Aspire.ApiService.Models;

public record SdkFeatureFlagsResponse(
    string Product,
    string Environment,
    List<SdkSectionResponse> Sections);

public record SdkSectionResponse(
    string Name,
    List<SdkFeatureKeyResponse> Keys);

public record SdkFeatureKeyResponse(
    string Name,
    FeatureKeyType Type,
    object? Value);
