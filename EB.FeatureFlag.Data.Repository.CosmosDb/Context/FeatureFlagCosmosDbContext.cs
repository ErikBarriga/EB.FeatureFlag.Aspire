using Microsoft.EntityFrameworkCore;
using EB.FeatureFlag.Data.Repository.CosmosDb.Entities;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Context;

public class FeatureFlagCosmosDbContext : DbContext
{
    public FeatureFlagCosmosDbContext(DbContextOptions<FeatureFlagCosmosDbContext> options)
        : base(options) { }

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<EnvironmentEntity> Environments => Set<EnvironmentEntity>();
    public DbSet<SectionEntity> Sections => Set<SectionEntity>();
    public DbSet<FeatureKeyEntity> FeatureKeys => Set<FeatureKeyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Product
        modelBuilder.Entity<ProductEntity>()
            .ToContainer("Products")
            .HasPartitionKey(p => p.Id);

        // Environment
        modelBuilder.Entity<EnvironmentEntity>()
            .ToContainer("Environments")
            .HasPartitionKey(e => e.ProductId);

        // Section
        modelBuilder.Entity<SectionEntity>()
            .ToContainer("Sections")
            .HasPartitionKey(s => s.ProductId);

        // FeatureKey
        modelBuilder.Entity<FeatureKeyEntity>()
            .ToContainer("FeatureKeys")
            .HasPartitionKey(fk => fk.ProductId);
    }
}