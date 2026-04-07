using Microsoft.EntityFrameworkCore;
using EB.FeatureFlag.Data.Repository.SQLite.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace EB.FeatureFlag.Data.Repository.SQLite.Context;

public class FeatureFlagSqliteDbContext : DbContext
{
    public FeatureFlagSqliteDbContext(DbContextOptions<FeatureFlagSqliteDbContext> options)
        : base(options) { }

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<EnvironmentEntity> Environments => Set<EnvironmentEntity>();
    public DbSet<SectionEntity> Sections => Set<SectionEntity>();
    public DbSet<FeatureKeyEntity> FeatureKeys => Set<FeatureKeyEntity>();
    public DbSet<ExternalSourceConfigEntity> ExternalSourceConfigs => Set<ExternalSourceConfigEntity>();
    public DbSet<ExternalSourceHeaderEntity> ExternalSourceHeaders => Set<ExternalSourceHeaderEntity>();

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

        // Map object? Value to string in database using JsonSerializer
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var objectConverter = new ValueConverter<object?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(v, jsonOptions),
            v => v == null ? null : JsonSerializer.Deserialize<object>(v, jsonOptions)
        );

        modelBuilder.Entity<FeatureKeyEntity>()
            .Property(fk => fk.Value)
            .HasConversion(objectConverter);

        modelBuilder.Entity<FeatureKeyEntity>()
            .HasOne(fk => fk.ExternalConfig)
            .WithOne(ec => ec.FeatureKey)
            .HasForeignKey<ExternalSourceConfigEntity>(ec => ec.FeatureKeyId)
            .OnDelete(DeleteBehavior.Cascade);

        // ExternalSourceConfig
        modelBuilder.Entity<ExternalSourceConfigEntity>()
            .HasKey(ec => ec.Id);
        modelBuilder.Entity<ExternalSourceConfigEntity>()
            .HasMany(ec => ec.Headers)
            .WithOne(h => h.Config)
            .HasForeignKey(h => h.ConfigId)
            .OnDelete(DeleteBehavior.Cascade);

        // ExternalSourceHeader
        modelBuilder.Entity<ExternalSourceHeaderEntity>()
            .HasKey(h => h.Id);
        modelBuilder.Entity<ExternalSourceHeaderEntity>()
            .HasIndex(h => h.ConfigId);
    }
}
