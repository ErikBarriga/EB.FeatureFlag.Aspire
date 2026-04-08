using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Repository.CosmosDb.Context;
using EB.FeatureFlag.Data.Repository.CosmosDb.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Repositories;

public class FeatureFlagDetailRepository : IFeatureFlagDetailRepository
{
    private readonly FeatureFlagCosmosDbContext _dbContext;

    public FeatureFlagDetailRepository(FeatureFlagCosmosDbContext dbContext) => _dbContext = dbContext;

    public async Task<FeatureFlagDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlagDetails.FindAsync(new object[] { id }, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IEnumerable<FeatureFlagDetailDto>> GetByFeatureFlagIdAsync(Guid featureFlagId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.FeatureFlagDetails
            .Where(d => d.FeatureFlagId == featureFlagId)
            .ToListAsync(cancellationToken);
        return entities.Select(d => d.ToDto());
    }

    public async Task<FeatureFlagDetailDto> AddAsync(FeatureFlagDetailDto detail, CancellationToken cancellationToken = default)
    {
        var entity = detail.ToEntity();
        _dbContext.FeatureFlagDetails.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task UpdateAsync(FeatureFlagDetailDto detail, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlagDetails.FindAsync(new object[] { detail.Id }, cancellationToken);
        if (entity == null) return;
        entity.Value = detail.Value;
        entity.ExternalConfig = detail.ExternalConfig;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlagDetails.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return;
        _dbContext.FeatureFlagDetails.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByFeatureFlagIdAsync(Guid featureFlagId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.FeatureFlagDetails
            .Where(d => d.FeatureFlagId == featureFlagId)
            .ToListAsync(cancellationToken);
        _dbContext.FeatureFlagDetails.RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
