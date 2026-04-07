using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Repository.CosmosDb.Context;
using EB.FeatureFlag.Data.Repository.CosmosDb.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly FeatureFlagCosmosDbContext _dbContext;

    public ProductRepository(FeatureFlagCosmosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Products.FindAsync(new object[] { id }, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Products.ToListAsync(cancellationToken);
        return entities.Select(e => e.ToDto());
    }

    public async Task<ProductDto> AddAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        var entity = product.ToEntity();
        _dbContext.Products.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task UpdateAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Products.FindAsync(new object[] { product.Id }, cancellationToken);
        if (entity == null) return;
        entity.Name = product.Name;
        entity.Description = product.Description;
        entity.Tags = product.Tags;
        entity.PrimaryAccessKey = product.PrimaryAccessKey;
        entity.SecondaryAccessKey = product.SecondaryAccessKey;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Products.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return;
        _dbContext.Products.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductDto?> GetByAccessKeyAsync(string accessKey, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.PrimaryAccessKey == accessKey || p.SecondaryAccessKey == accessKey, cancellationToken);
        return entity?.ToDto();
    }
}