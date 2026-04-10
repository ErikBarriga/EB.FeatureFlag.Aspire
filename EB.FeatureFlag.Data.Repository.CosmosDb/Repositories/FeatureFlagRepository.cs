using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Repository.CosmosDb.Context;
using EB.FeatureFlag.Data.Repository.CosmosDb.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Repositories;

public class FeatureFlagRepository : IFeatureFlagRepository
{
    private readonly FeatureFlagCosmosDbContext _dbContext;

    public FeatureFlagRepository(FeatureFlagCosmosDbContext dbContext) => _dbContext = dbContext;

    public async Task<FeatureFlagDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlags.FindAsync(new object[] { id }, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IEnumerable<FeatureFlagDto>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.FeatureFlags
            .Where(ff => ff.SectionId == sectionId)
            .ToListAsync(cancellationToken);
        return entities.Select(ff => ff.ToDto());
    }

    public async Task<IEnumerable<FeatureFlagDto>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.FeatureFlags
            .Where(ff => ff.ProductId == productId)
            .ToListAsync(cancellationToken);
        return entities.Select(ff => ff.ToDto());
    }

    public async Task<FeatureFlagDto?> GetByProductIdAndKeyAsync(Guid productId, string key, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlags
            .FirstOrDefaultAsync(ff => ff.ProductId == productId && ff.Key == key, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<FeatureFlagDto> AddAsync(FeatureFlagDto featureFlag, CancellationToken cancellationToken = default)
    {
        var entity = featureFlag.ToEntity();
        _dbContext.FeatureFlags.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task UpdateAsync(FeatureFlagDto featureFlag, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlags.FindAsync(new object[] { featureFlag.Id }, cancellationToken);
        if (entity == null) return;
        entity.Key = featureFlag.Key;
        entity.Description = featureFlag.Description;
        entity.Tags = featureFlag.Tags;
        entity.ValidationRegex = featureFlag.ValidationRegex;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlags.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return;
        _dbContext.FeatureFlags.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
