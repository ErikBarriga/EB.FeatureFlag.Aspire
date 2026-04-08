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
    public DbSet<FeatureFlagEntity> FeatureFlags => Set<FeatureFlagEntity>();
    public DbSet<FeatureFlagDetailEntity> FeatureFlagDetails => Set<FeatureFlagDetailEntity>();
    public DbSet<ExternalSourceConfigEntity> ExternalSourceConfigs => Set<ExternalSourceConfigEntity>();
    public DbSet<ExternalSourceHeaderEntity> ExternalSourceHeaders => Set<ExternalSourceHeaderEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>().HasKey(p => p.Id);

        modelBuilder.Entity<EnvironmentEntity>().HasKey(e => e.Id);
        modelBuilder.Entity<EnvironmentEntity>().HasIndex(e => e.ProductId);

        modelBuilder.Entity<SectionEntity>().HasKey(s => s.Id);
        modelBuilder.Entity<SectionEntity>().HasIndex(s => s.ProductId);

        modelBuilder.Entity<FeatureFlagEntity>().HasKey(ff => ff.Id);
        modelBuilder.Entity<FeatureFlagEntity>().HasIndex(ff => new { ff.ProductId, ff.SectionId });

        modelBuilder.Entity<FeatureFlagDetailEntity>().HasKey(d => d.Id);
        modelBuilder.Entity<FeatureFlagDetailEntity>().HasIndex(d => new { d.ProductId, d.FeatureFlagId, d.EnvironmentId });

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var objectConverter = new ValueConverter<object?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(v, jsonOptions),
            v => v == null ? null : JsonSerializer.Deserialize<object>(v, jsonOptions)
        );

        modelBuilder.Entity<FeatureFlagDetailEntity>()
            .Property(d => d.Value)
            .HasConversion(objectConverter);

        modelBuilder.Entity<FeatureFlagDetailEntity>()
            .HasOne(d => d.ExternalConfig)
            .WithOne(ec => ec.FeatureFlagDetail)
            .HasForeignKey<ExternalSourceConfigEntity>(ec => ec.FeatureFlagDetailId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExternalSourceConfigEntity>().HasKey(ec => ec.Id);
        modelBuilder.Entity<ExternalSourceConfigEntity>()
            .HasMany(ec => ec.Headers)
            .WithOne(h => h.Config)
            .HasForeignKey(h => h.ConfigId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExternalSourceHeaderEntity>().HasKey(h => h.Id);
        modelBuilder.Entity<ExternalSourceHeaderEntity>().HasIndex(h => h.ConfigId);
    }
}
