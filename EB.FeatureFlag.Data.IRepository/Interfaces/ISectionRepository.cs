using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Data.IRepository.Interfaces;

public interface ISectionRepository
{
    Task<SectionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SectionDto>> GetByEnvironmentIdAsync(Guid environmentId, CancellationToken cancellationToken = default);
    Task<SectionDto> AddAsync(SectionDto section, CancellationToken cancellationToken = default);
    Task UpdateAsync(SectionDto section, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}