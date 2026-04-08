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
    public DbSet<FeatureFlagEntity> FeatureFlags => Set<FeatureFlagEntity>();
    public DbSet<FeatureFlagDetailEntity> FeatureFlagDetails => Set<FeatureFlagDetailEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>()
            .ToContainer("Products")
            .HasPartitionKey(p => p.Id);

        modelBuilder.Entity<EnvironmentEntity>()
            .ToContainer("Environments")
            .HasPartitionKey(e => e.ProductId);

        modelBuilder.Entity<SectionEntity>()
            .ToContainer("Sections")
            .HasPartitionKey(s => s.ProductId);

        modelBuilder.Entity<FeatureFlagEntity>()
            .ToContainer("FeatureFlags")
            .HasPartitionKey(ff => ff.ProductId);

        modelBuilder.Entity<FeatureFlagDetailEntity>()
            .ToContainer("FeatureFlagDetails")
            .HasPartitionKey(d => d.ProductId);
    }
}
