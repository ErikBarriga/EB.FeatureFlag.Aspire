using EB.FeatureFlag.Data.Cache.InMemory;
using EB.FeatureFlag.Data.Cache.Redis;
using EB.FeatureFlag.Data.ICache;
using EB.FeatureFlag.Data.IProvider;
using EB.FeatureFlag.Data.IProvider.ExternalSource;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Interfaces;
using EB.FeatureFlag.Data.Provider;
using EB.FeatureFlag.Data.Provider.ExternalSource;
using EB.FeatureFlag.Data.Provider.Validators;
using EB.FeatureFlag.Data.Repository.CosmosDb.Context;
using EB.FeatureFlag.Data.Repository.CosmosDb.Repositories;
using EB.FeatureFlag.Data.Repository.SQLite.Context;
using EB.FeatureFlag.Data.Repository.SQLite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EB.FeatureFlag.Data;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Feature Flag data services: repository, cache (optional), and provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="FeatureFlagDataOptions"/>.</param>
    /// <example>
    /// // Cosmos DB + Redis cache:
    /// builder.Services.AddFeatureFlagData(options =>
    /// {
    ///     options.RepositoryType = FeatureFlagRepositoryType.Cosmos;
    ///     options.RepositoryConnectionString = "AccountEndpoint=https://...;AccountKey=...";
    ///     options.CacheType = FeatureFlagCacheType.Redis;
    ///     options.CacheConnectionString = "localhost:6379";
    /// });
    ///
    /// // Cosmos DB + In-Memory cache (no connection string needed):
    /// builder.Services.AddFeatureFlagData(options =>
    /// {
    ///     options.RepositoryType = FeatureFlagRepositoryType.Cosmos;
    ///     options.RepositoryConnectionString = "AccountEndpoint=https://...;AccountKey=...";
    ///     options.CacheType = FeatureFlagCacheType.InMemory;
    /// });
    ///
    /// // Cosmos DB without cache:
    /// builder.Services.AddFeatureFlagData(options =>
    /// {
    ///     options.RepositoryType = FeatureFlagRepositoryType.Cosmos;
    ///     options.RepositoryConnectionString = "AccountEndpoint=https://...;AccountKey=...";
    ///     // CacheType defaults to FeatureFlagCacheType.None
    /// });
    /// </example>
    public static IServiceCollection AddFeatureFlagData(
        this IServiceCollection services,
        Action<FeatureFlagDataOptions> configure)
    {
        var options = new FeatureFlagDataOptions();
        configure(options);

        services.AddFeatureFlagRepository(options);
        services.AddFeatureFlagCache(options);
        services.AddFeatureFlagValidators();
        services.AddFeatureFlagProvider();

        return services;
    }

    private static IServiceCollection AddFeatureFlagRepository(
        this IServiceCollection services,
        FeatureFlagDataOptions options)
    {
        switch (options.RepositoryType)
        {
            case FeatureFlagRepositoryType.Cosmos:
                services.AddDbContext<FeatureFlagCosmosDbContext>(dbOptions =>
                    dbOptions.UseCosmos(
                        options.RepositoryConnectionString,
                        databaseName: "FeatureFlagDb"));

                services.AddScoped<IProductRepository, Repository.CosmosDb.Repositories.ProductRepository>();
                services.AddScoped<IEnvironmentRepository, Repository.CosmosDb.Repositories.EnvironmentRepository>();
                services.AddScoped<ISectionRepository, Repository.CosmosDb.Repositories.SectionRepository>();
                services.AddScoped<IFeatureKeyRepository, Repository.CosmosDb.Repositories.FeatureKeyRepository>();
                break;

            case FeatureFlagRepositoryType.SQLite:
                services.AddDbContext<FeatureFlagSqliteDbContext>(dbOptions =>
                    dbOptions.UseSqlite(
                        options.RepositoryConnectionString ?? "Data Source=featureflag.db"));

                services.AddScoped<IProductRepository, Repository.SQLite.Repositories.ProductRepository>();
                services.AddScoped<IEnvironmentRepository, Repository.SQLite.Repositories.EnvironmentRepository>();
                services.AddScoped<ISectionRepository, Repository.SQLite.Repositories.SectionRepository>();
                services.AddScoped<IFeatureKeyRepository, Repository.SQLite.Repositories.FeatureKeyRepository>();
                break;

            default:
                throw new NotSupportedException(
                    $"Repository type '{options.RepositoryType}' is not supported.");
        }

        return services;
    }

    private static IServiceCollection AddFeatureFlagCache(
        this IServiceCollection services,
        FeatureFlagDataOptions options)
    {
        switch (options.CacheType)
        {
            case FeatureFlagCacheType.None:
                break;

            case FeatureFlagCacheType.InMemory:
                services.AddSingleton<ICacheService, InMemoryCacheService>();
                break;

            case FeatureFlagCacheType.Redis:
                services.AddSingleton<ICacheService>(
                    _ => new RedisCacheService(options.CacheConnectionString));
                break;

            default:
                throw new NotSupportedException(
                    $"Cache type '{options.CacheType}' is not supported.");
        }

        return services;
    }

    private static IServiceCollection AddFeatureFlagValidators(this IServiceCollection services)
    {
        services.AddSingleton<IFeatureKeyValueValidator, BooleanValueValidator>();
        services.AddSingleton<IFeatureKeyValueValidator, LargeStringValueValidator>();
        services.AddSingleton<IFeatureKeyValueValidator, StringCollectionValueValidator>();
        services.AddSingleton<IFeatureKeyValueValidator, JsonCollectionValueValidator>();
        services.AddSingleton<IFeatureKeyValueValidatorFactory, FeatureKeyValueValidatorFactory>();

        return services;
    }

    private static IServiceCollection AddFeatureFlagProvider(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IExternalSourceService, ExternalSourceService>();

        services.AddScoped<IFeatureFlagProvider>(sp =>
        {
            var productRepo = sp.GetRequiredService<IProductRepository>();
            var environmentRepo = sp.GetRequiredService<IEnvironmentRepository>();
            var sectionRepo = sp.GetRequiredService<ISectionRepository>();
            var featureKeyRepo = sp.GetRequiredService<IFeatureKeyRepository>();
            var cacheService = sp.GetService<ICacheService>();
            var validatorFactory = sp.GetService<IFeatureKeyValueValidatorFactory>();
            var externalSourceService = sp.GetService<IExternalSourceService>();

            return new FeatureFlagProvider(
                productRepo, environmentRepo, sectionRepo, featureKeyRepo, cacheService, validatorFactory, externalSourceService);
        });

        return services;
    }
}
