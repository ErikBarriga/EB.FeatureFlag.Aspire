using Microsoft.EntityFrameworkCore;
using EB.FeatureFlag.Data.Repository.SQLite.Entities;

namespace EB.FeatureFlag.Data.Repository.SQLite.Context;

public class FeatureFlagSqliteDbContext : DbContext
{
    public FeatureFlagSqliteDbContext(DbContextOptions<FeatureFlagSqliteDbContext> options)
        : base(options) { }

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<EnvironmentEntity> Environments => Set<EnvironmentEntity>();
    public DbSet<SectionEntity> Sections => Set<SectionEntity>();
    public DbSet<FeatureKeyEntity> FeatureKeys => Set<FeatureKeyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product
        modelBuilder.Entity<ProductEntity>()
            .HasKey(p => p.Id);

        // Environment
        modelBuilder.Entity<EnvironmentEntity>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<EnvironmentEntity>()
            .HasIndex(e => e.ProductId);

        // Section
        modelBuilder.Entity<SectionEntity>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<SectionEntity>()
            .HasIndex(s => new { s.ProductId, s.EnvironmentId });

        // FeatureKey
        modelBuilder.Entity<FeatureKeyEntity>()
            .HasKey(fk => fk.Id);
        modelBuilder.Entity<FeatureKeyEntity>()
            .HasIndex(fk => new { fk.ProductId, fk.EnvironmentId, fk.SectionId });
    }
}
