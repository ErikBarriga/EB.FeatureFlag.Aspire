using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Repository.SQLite.Context;
using EB.FeatureFlag.Data.Repository.SQLite.Entities;
using EB.FeatureFlag.Data.Repository.SQLite.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EB.FeatureFlag.Data.Repository.SQLite.Repositories;

public class FeatureFlagDetailRepository : IFeatureFlagDetailRepository
{
    private readonly FeatureFlagSqliteDbContext _dbContext;

    public FeatureFlagDetailRepository(FeatureFlagSqliteDbContext dbContext) => _dbContext = dbContext;

    public async Task<FeatureFlagDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlagDetails
            .Include(d => d.ExternalConfig)
            .ThenInclude(ec => ec.Headers)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IEnumerable<FeatureFlagDetailDto>> GetByFeatureFlagIdAsync(Guid featureFlagId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.FeatureFlagDetails
            .Where(d => d.FeatureFlagId == featureFlagId)
            .Include(d => d.ExternalConfig)
            .ThenInclude(ec => ec.Headers)
            .ToListAsync(cancellationToken);
        return entities.Select(d => d.ToDto());
    }

    public async Task<FeatureFlagDetailDto?> GetByFeatureFlagIdAndEnvironmentIdAsync(Guid featureFlagId, Guid environmentId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FeatureFlagDetails
            .Include(d => d.ExternalConfig)
            .ThenInclude(ec => ec.Headers)
            .FirstOrDefaultAsync(d => d.FeatureFlagId == featureFlagId && d.EnvironmentId == environmentId, cancellationToken);
        return entity?.ToDto();
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
        var entity = await _dbContext.FeatureFlagDetails
            .Include(d => d.ExternalConfig)
            .ThenInclude(ec => ec.Headers)
            .FirstOrDefaultAsync(d => d.Id == detail.Id, cancellationToken);
        if (entity == null) return;

        entity.Value = detail.Value;

        if (detail.ExternalConfig != null)
        {
            if (entity.ExternalConfig == null)
            {
                entity.ExternalConfig = detail.ExternalConfig.ToEntity(entity.Id);
            }
            else
            {
                entity.ExternalConfig.Endpoint = detail.ExternalConfig.Url;
                if (detail.ExternalConfig.Headers != null)
                {
                    _dbContext.ExternalSourceHeaders.RemoveRange(entity.ExternalConfig.Headers);
                    entity.ExternalConfig.Headers = detail.ExternalConfig.Headers
                        .Select(kvp => new ExternalSourceHeaderEntity
                        {
                            Id = Guid.NewGuid(),
                            ConfigId = entity.ExternalConfig.Id,
                            Key = kvp.Key,
                            Value = kvp.Value
                        }).ToList();
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
