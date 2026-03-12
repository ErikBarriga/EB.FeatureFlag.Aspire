using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Repository.CosmosDb.Context;
using EB.FeatureFlag.Data.Repository.CosmosDb.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Repositories;

public class FeatureKeyRepository : IFeatureKeyRepository
{
    private readonly FeatureFlagCosmosDbContext _dbContext;

    public FeatureKeyRepository(FeatureFlagCosmosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FeatureKeyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureKeys.FindAsync(new object[] { id }, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IEnumerable<FeatureKeyDto>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.FeatureKeys
            .Where(fk => fk.SectionId == sectionId)
            .ToListAsync(cancellationToken);
        return entities.Select(fk => fk.ToDto());
    }

    public async Task<FeatureKeyDto> AddAsync(FeatureKeyDto featureKey, CancellationToken cancellationToken = default)
    {
        // EnvironmentId and ProductId are required for partitioning; you may need to pass them explicitly if not present in DTO
        var entity = featureKey.ToEntity(Guid.Empty, Guid.Empty); // Replace Guid.Empty with actual EnvironmentId and ProductId if available
        _dbContext.FeatureKeys.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task UpdateAsync(FeatureKeyDto featureKey, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureKeys.FindAsync(new object[] { featureKey.Id }, cancellationToken);
        if (entity == null) return;
        entity.SectionId = featureKey.SectionId;
        entity.Name = featureKey.Name;
        entity.Description = featureKey.Description;
        entity.Tags = featureKey.Tags;
        entity.Type = featureKey.Type;
        entity.Value = featureKey.Value;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureKeys.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return;
        _dbContext.FeatureKeys.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}