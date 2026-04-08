using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Data.IRepository.Interfaces;

public interface IFeatureFlagRepository
{
    Task<FeatureFlagDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureFlagDto>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureFlagDto>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<FeatureFlagDto> AddAsync(FeatureFlagDto featureFlag, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeatureFlagDto featureFlag, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
