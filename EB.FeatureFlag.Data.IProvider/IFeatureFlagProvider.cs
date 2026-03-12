using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Data.IProvider;

/// <summary>
/// Provides unified access to Feature Flag data, abstracting repository and cache implementations.
/// Supports read, upsert, and delete operations for all entities.
/// </summary>
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

    // Section operations
    Task<SectionDto?> GetSectionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SectionDto>> GetSectionsByEnvironmentIdAsync(Guid environmentId, CancellationToken cancellationToken = default);
    Task<SectionDto> UpsertSectionAsync(SectionDto section, CancellationToken cancellationToken = default);
    Task DeleteSectionAsync(Guid id, CancellationToken cancellationToken = default);

    // Feature Key operations
    Task<FeatureKeyDto?> GetFeatureKeyByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureKeyDto>> GetFeatureKeysBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<FeatureKeyDto> UpsertFeatureKeyAsync(FeatureKeyDto featureKey, CancellationToken cancellationToken = default);
    Task DeleteFeatureKeyAsync(Guid id, CancellationToken cancellationToken = default);
}