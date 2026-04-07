using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Repository.SQLite.Context;
using EB.FeatureFlag.Data.Repository.SQLite.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EB.FeatureFlag.Data.Repository.SQLite.Repositories;

public class SectionRepository : ISectionRepository
{
    private readonly FeatureFlagSqliteDbContext _dbContext;

    public SectionRepository(FeatureFlagSqliteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SectionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Sections.FindAsync(new object[] { id }, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IEnumerable<SectionDto>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Sections
            .Where(s => s.ProductId == productId)
            .ToListAsync(cancellationToken);
        return entities.Select(s => s.ToDto());
    }

    public async Task<SectionDto> AddAsync(SectionDto section, CancellationToken cancellationToken = default)
    {
        var entity = section.ToEntity();
        _dbContext.Sections.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task UpdateAsync(SectionDto section, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Sections.FindAsync(new object[] { section.Id }, cancellationToken);
        if (entity == null) return;
        entity.Name = section.Name;
        entity.Description = section.Description;
        entity.Tags = section.Tags;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Sections.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return;
        _dbContext.Sections.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
