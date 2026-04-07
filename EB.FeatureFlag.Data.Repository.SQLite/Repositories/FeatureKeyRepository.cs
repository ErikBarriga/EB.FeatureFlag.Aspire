using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Repository.SQLite.Context;
using EB.FeatureFlag.Data.Repository.SQLite.Entities;
using EB.FeatureFlag.Data.Repository.SQLite.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EB.FeatureFlag.Data.Repository.SQLite.Repositories;

public class FeatureKeyRepository : IFeatureKeyRepository
{
    private readonly FeatureFlagSqliteDbContext _dbContext;

    public FeatureKeyRepository(FeatureFlagSqliteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FeatureKeyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureKeys
            .Include(fk => fk.ExternalConfig)
            .ThenInclude(ec => ec.Headers)
            .FirstOrDefaultAsync(fk => fk.Id == id, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IEnumerable<FeatureKeyDto>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.FeatureKeys
            .Where(fk => fk.SectionId == sectionId)
            .Include(fk => fk.ExternalConfig)
            .ThenInclude(ec => ec.Headers)
            .ToListAsync(cancellationToken);
        return entities.Select(fk => fk.ToDto());
    }

    public async Task<FeatureKeyDto> AddAsync(FeatureKeyDto featureKey, Guid environmentId, Guid productId, CancellationToken cancellationToken = default)
    {
        var entity = featureKey.ToEntity(environmentId, productId);
        _dbContext.FeatureKeys.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task UpdateAsync(FeatureKeyDto featureKey, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureKeys
            .Include(fk => fk.ExternalConfig)
            .ThenInclude(ec => ec.Headers)
            .FirstOrDefaultAsync(fk => fk.Id == featureKey.Id, cancellationToken);
        if (entity == null) return;

        entity.SectionId = featureKey.SectionId;
        entity.Name = featureKey.Name;
        entity.Description = featureKey.Description;
        entity.Tags = featureKey.Tags;
        entity.Type = featureKey.Type;
        entity.Value = featureKey.Value;
        entity.ValidationRegex = featureKey.ValidationRegex;

        // Handle ExternalConfig conversion
        if (featureKey.ExternalConfig != null)
        {
            if (entity.ExternalConfig == null)
            {
                entity.ExternalConfig = featureKey.ExternalConfig.ToEntity(entity.Id);
            }
            else
            {
                // Update existing config
                entity.ExternalConfig.Endpoint = featureKey.ExternalConfig.Url;

                // Update headers
                if (featureKey.ExternalConfig.Headers != null)
                {
                    _dbContext.ExternalSourceHeaders.RemoveRange(entity.ExternalConfig.Headers);
                    entity.ExternalConfig.Headers = featureKey.ExternalConfig.Headers
                        .Select(kvp => new ExternalSourceHeaderEntity
                        {
                            Id = Guid.NewGuid(),
                            ConfigId = entity.ExternalConfig.Id,
                            Key = kvp.Key,
                            Value = kvp.Value
                        })
                        .ToList();
                }
            }
        }
        else if (entity.ExternalConfig != null)
        {
            _dbContext.ExternalSourceConfigs.Remove(entity.ExternalConfig);
        }

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
