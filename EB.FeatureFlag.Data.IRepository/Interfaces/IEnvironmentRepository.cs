using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Data.IRepository.Interfaces;

public interface IEnvironmentRepository
{
    Task<EnvironmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EnvironmentDto>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<EnvironmentDto> AddAsync(EnvironmentDto environment, CancellationToken cancellationToken = default);
    Task UpdateAsync(EnvironmentDto environment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}