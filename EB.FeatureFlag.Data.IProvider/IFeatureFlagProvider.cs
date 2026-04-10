using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Data.IProvider;

public interface IFeatureFlagProvider
{
    // Product operations
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductDto> UpsertProductAsync(ProductDto product, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);

    // Environment operations
    Task<EnvironmentDto?> GetEnvironmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EnvironmentDto>> GetEnvironmentsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<EnvironmentDto> UpsertEnvironmentAsync(EnvironmentDto environment, CancellationToken cancellationToken = default);
    Task DeleteEnvironmentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EnvironmentDto> RotateEnvironmentKeysAsync(Guid environmentId, string keyType, CancellationToken cancellationToken = default);

    // Section operations
    Task<SectionDto?> GetSectionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SectionDto>> GetSectionsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<SectionDto> UpsertSectionAsync(SectionDto section, CancellationToken cancellationToken = default);
    Task DeleteSectionAsync(Guid id, CancellationToken cancellationToken = default);

    // FeatureFlag operations (definition)
    Task<FeatureFlagDto?> GetFeatureFlagByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureFlagDto>> GetFeatureFlagsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureFlagDto>> GetFeatureFlagsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<FeatureFlagDto> UpsertFeatureFlagAsync(FeatureFlagDto featureFlag, CancellationToken cancellationToken = default);
    Task DeleteFeatureFlagAsync(Guid id, CancellationToken cancellationToken = default);

    // FeatureFlagDetail operations (per-environment values)
    Task<FeatureFlagDetailDto?> GetFeatureFlagDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureFlagDetailDto>> GetFeatureFlagDetailsByFlagIdAsync(Guid featureFlagId, CancellationToken cancellationToken = default);
    Task<FeatureFlagDetailDto> UpsertFeatureFlagDetailAsync(FeatureFlagDetailDto detail, FeatureFlagDto featureFlag, CancellationToken cancellationToken = default);

    // SDK / Public API
    Task<(string Product, string Environment, string Section, SdkFeatureFlagItemDto Flag)?> GetFeatureFlagByKeyAndAccessKeyAsync(
        string environmentKey, string flagKey, CancellationToken cancellationToken = default);

    Task<SdkFeatureFlagValueDto?> GetFeatureFlagValueByKeyAndAccessKeyAsync(
        string environmentKey, string flagKey, CancellationToken cancellationToken = default);
}
