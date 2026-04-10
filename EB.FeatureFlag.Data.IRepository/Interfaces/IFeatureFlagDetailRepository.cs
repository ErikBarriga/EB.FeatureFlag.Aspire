using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Data.IRepository.Interfaces;

public interface IFeatureFlagDetailRepository
{
    Task<FeatureFlagDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeatureFlagDetailDto>> GetByFeatureFlagIdAsync(Guid featureFlagId, CancellationToken cancellationToken = default);
    Task<FeatureFlagDetailDto?> GetByFeatureFlagIdAndEnvironmentIdAsync(Guid featureFlagId, Guid environmentId, CancellationToken cancellationToken = default);
    Task<FeatureFlagDetailDto> AddAsync(FeatureFlagDetailDto detail, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeatureFlagDetailDto detail, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteByFeatureFlagIdAsync(Guid featureFlagId, CancellationToken cancellationToken = default);
}
