using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Data.IRepository.Interfaces;

public interface IFeatureKeyRepository
{
    Task<FeatureKeyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureKeyDto>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<FeatureKeyDto> AddAsync(FeatureKeyDto featureKey, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeatureKeyDto featureKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}