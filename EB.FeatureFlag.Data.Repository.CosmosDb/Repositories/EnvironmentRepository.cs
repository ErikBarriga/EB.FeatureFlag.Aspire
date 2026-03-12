using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Repository.CosmosDb.Context;
using EB.FeatureFlag.Data.Repository.CosmosDb.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Repositories;

public class EnvironmentRepository : IEnvironmentRepository
{
    private readonly FeatureFlagCosmosDbContext _dbContext;

    public EnvironmentRepository(FeatureFlagCosmosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EnvironmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Environments.FindAsync(new object[] { id }, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IEnumerable<EnvironmentDto>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Environments
            .Where(e => e.ProductId == productId)
            .ToListAsync(cancellationToken);
        return entities.Select(e => e.ToDto());
    }

    public async Task<EnvironmentDto> AddAsync(EnvironmentDto environment, CancellationToken cancellationToken = default)
    {
        var entity = environment.ToEntity();
        _dbContext.Environments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task UpdateAsync(EnvironmentDto environment, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Environments.FindAsync(new object[] { environment.Id }, cancellationToken);
        if (entity == null) return;
        entity.ProductId = environment.ProductId;
        entity.Name = environment.Name;
        entity.Description = environment.Description;
        entity.Tags = environment.Tags;
        entity.PrimaryAccessKey = environment.PrimaryAccessKey;
        entity.SecondaryAccessKey = environment.SecondaryAccessKey;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Environments.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return;
        _dbContext.Environments.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}