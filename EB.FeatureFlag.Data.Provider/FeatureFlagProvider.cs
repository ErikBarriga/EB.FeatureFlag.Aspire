using EB.FeatureFlag.Data.ICache;
using EB.FeatureFlag.Data.IProvider;
using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;

namespace EB.FeatureFlag.Data.Provider;

public class FeatureFlagProvider : IFeatureFlagProvider
{
    private readonly IProductRepository _productRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IFeatureKeyRepository _featureKeyRepository;
    private readonly ICacheService? _cacheService;
    private static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMinutes(10);

    public FeatureFlagProvider(
        IProductRepository productRepository,
        IEnvironmentRepository environmentRepository,
        ISectionRepository sectionRepository,
        IFeatureKeyRepository featureKeyRepository,
        ICacheService? cacheService = null)
    {
        _productRepository = productRepository;
        _environmentRepository = environmentRepository;
        _sectionRepository = sectionRepository;
        _featureKeyRepository = featureKeyRepository;
        _cacheService = cacheService;
    }

    // --- Product ---
    private static string GetProductCacheKey(Guid id) => $"FeatureFlag:Product:{id}";
    private static string GetAllProductsCacheKey() => $"FeatureFlag:Product:All";

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProductCacheKey(id);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<ProductDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product != null && _cacheService != null)
            await _cacheService.SetAsync(cacheKey, product, DefaultCacheExpiration, cancellationToken);

        return product;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetAllProductsCacheKey();
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var products = await _productRepository.GetAllAsync(cancellationToken);
        if (_cacheService != null)
            await _cacheService.SetAsync(cacheKey, products, DefaultCacheExpiration, cancellationToken);

        return products;
    }

    public async Task<ProductDto> UpsertProductAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        ProductDto result;
        if (product.Id == Guid.Empty)
        {
            if (string.IsNullOrEmpty(product.PrimaryAccessKey))
                product.PrimaryAccessKey = AccessKeyGenerator.GenerateAccessKey();
            if (string.IsNullOrEmpty(product.SecondaryAccessKey))
                product.SecondaryAccessKey = AccessKeyGenerator.GenerateAccessKey();

            result = await _productRepository.AddAsync(product, cancellationToken);
        }
        else
        {
            await _productRepository.UpdateAsync(product, cancellationToken);
            result = product;
        }

        if (_cacheService != null)
        {
            var cacheKey = GetProductCacheKey(result.Id);
            await _cacheService.SetAsync(cacheKey, result, DefaultCacheExpiration, cancellationToken);
            await _cacheService.RemoveAsync(GetAllProductsCacheKey(), cancellationToken);
        }

        return result;
    }

    public async Task<ProductDto> RotateProductKeysAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{productId}' not found.");

        product.SecondaryAccessKey = product.PrimaryAccessKey;
        product.PrimaryAccessKey = AccessKeyGenerator.GenerateAccessKey();

        await _productRepository.UpdateAsync(product, cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(GetProductCacheKey(productId), product, DefaultCacheExpiration, cancellationToken);
            await _cacheService.RemoveAsync(GetAllProductsCacheKey(), cancellationToken);
        }

        return product;
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _productRepository.DeleteAsync(id, cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(GetProductCacheKey(id), cancellationToken);
            await _cacheService.RemoveAsync(GetAllProductsCacheKey(), cancellationToken);
        }
    }

    // --- Environment ---
    private static string GetEnvironmentCacheKey(Guid id) => $"FeatureFlag:Environment:{id}";
    private static string GetEnvironmentsByProductCacheKey(Guid productId) => $"FeatureFlag:Environment:ByProduct:{productId}";

    public async Task<EnvironmentDto?> GetEnvironmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetEnvironmentCacheKey(id);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<EnvironmentDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var environment = await _environmentRepository.GetByIdAsync(id, cancellationToken);
        if (environment != null && _cacheService != null)
            await _cacheService.SetAsync(cacheKey, environment, DefaultCacheExpiration, cancellationToken);

        return environment;
    }

    public async Task<IEnumerable<EnvironmentDto>> GetEnvironmentsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetEnvironmentsByProductCacheKey(productId);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<IEnumerable<EnvironmentDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var environments = await _environmentRepository.GetByProductIdAsync(productId, cancellationToken);
        if (_cacheService != null)
            await _cacheService.SetAsync(cacheKey, environments, DefaultCacheExpiration, cancellationToken);

        return environments;
    }

    public async Task<EnvironmentDto> UpsertEnvironmentAsync(EnvironmentDto environment, CancellationToken cancellationToken = default)
    {
        EnvironmentDto result;
        if (environment.Id == Guid.Empty)
        {
            if (string.IsNullOrEmpty(environment.PrimaryAccessKey))
                environment.PrimaryAccessKey = AccessKeyGenerator.GenerateAccessKey();
            if (string.IsNullOrEmpty(environment.SecondaryAccessKey))
                environment.SecondaryAccessKey = AccessKeyGenerator.GenerateAccessKey();

            result = await _environmentRepository.AddAsync(environment, cancellationToken);
        }
        else
        {
            await _environmentRepository.UpdateAsync(environment, cancellationToken);
            result = environment;
        }

        if (_cacheService != null)
        {
            var cacheKey = GetEnvironmentCacheKey(result.Id);
            await _cacheService.SetAsync(cacheKey, result, DefaultCacheExpiration, cancellationToken);
            await _cacheService.RemoveAsync(GetEnvironmentsByProductCacheKey(result.ProductId), cancellationToken);
        }

        return result;
    }

    public async Task<EnvironmentDto> RotateEnvironmentKeysAsync(Guid environmentId, CancellationToken cancellationToken = default)
    {
        var environment = await _environmentRepository.GetByIdAsync(environmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Environment '{environmentId}' not found.");

        environment.SecondaryAccessKey = environment.PrimaryAccessKey;
        environment.PrimaryAccessKey = AccessKeyGenerator.GenerateAccessKey();

        await _environmentRepository.UpdateAsync(environment, cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(GetEnvironmentCacheKey(environmentId), environment, DefaultCacheExpiration, cancellationToken);
            await _cacheService.RemoveAsync(GetEnvironmentsByProductCacheKey(environment.ProductId), cancellationToken);
        }

        return environment;
    }

    public async Task DeleteEnvironmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var environment = await _environmentRepository.GetByIdAsync(id, cancellationToken);
        if (environment == null)
            return;

        await _environmentRepository.DeleteAsync(id, cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(GetEnvironmentCacheKey(id), cancellationToken);
            await _cacheService.RemoveAsync(GetEnvironmentsByProductCacheKey(environment.ProductId), cancellationToken);
        }
    }

    // --- Section ---
    private static string GetSectionCacheKey(Guid id) => $"FeatureFlag:Section:{id}";
    private static string GetSectionsByEnvironmentCacheKey(Guid environmentId) => $"FeatureFlag:Section:ByEnvironment:{environmentId}";

    public async Task<SectionDto?> GetSectionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetSectionCacheKey(id);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<SectionDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var section = await _sectionRepository.GetByIdAsync(id, cancellationToken);
        if (section != null && _cacheService != null)
            await _cacheService.SetAsync(cacheKey, section, DefaultCacheExpiration, cancellationToken);

        return section;
    }

    public async Task<IEnumerable<SectionDto>> GetSectionsByEnvironmentIdAsync(Guid environmentId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetSectionsByEnvironmentCacheKey(environmentId);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<IEnumerable<SectionDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var sections = await _sectionRepository.GetByEnvironmentIdAsync(environmentId, cancellationToken);
        if (_cacheService != null)
            await _cacheService.SetAsync(cacheKey, sections, DefaultCacheExpiration, cancellationToken);

        return sections;
    }

    public async Task<SectionDto> UpsertSectionAsync(SectionDto section, CancellationToken cancellationToken = default)
    {
        SectionDto result;
        if (section.Id == Guid.Empty)
        {
            result = await _sectionRepository.AddAsync(section, cancellationToken);
        }
        else
        {
            await _sectionRepository.UpdateAsync(section, cancellationToken);
            result = section;
        }

        if (_cacheService != null)
        {
            var cacheKey = GetSectionCacheKey(result.Id);
            await _cacheService.SetAsync(cacheKey, result, DefaultCacheExpiration, cancellationToken);
            await _cacheService.RemoveAsync(GetSectionsByEnvironmentCacheKey(result.EnvironmentId), cancellationToken);
        }

        return result;
    }

    public async Task DeleteSectionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var section = await _sectionRepository.GetByIdAsync(id, cancellationToken);
        if (section == null)
            return;

        await _sectionRepository.DeleteAsync(id, cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(GetSectionCacheKey(id), cancellationToken);
            await _cacheService.RemoveAsync(GetSectionsByEnvironmentCacheKey(section.EnvironmentId), cancellationToken);
        }
    }

    // --- FeatureKey ---
    private static string GetFeatureKeyCacheKey(Guid id) => $"FeatureFlag:FeatureKey:{id}";
    private static string GetFeatureKeysBySectionCacheKey(Guid sectionId) => $"FeatureFlag:FeatureKey:BySection:{sectionId}";

    public async Task<FeatureKeyDto?> GetFeatureKeyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetFeatureKeyCacheKey(id);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<FeatureKeyDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var featureKey = await _featureKeyRepository.GetByIdAsync(id, cancellationToken);
        if (featureKey != null && _cacheService != null)
            await _cacheService.SetAsync(cacheKey, featureKey, DefaultCacheExpiration, cancellationToken);

        return featureKey;
    }

    public async Task<IEnumerable<FeatureKeyDto>> GetFeatureKeysBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetFeatureKeysBySectionCacheKey(sectionId);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<IEnumerable<FeatureKeyDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var featureKeys = await _featureKeyRepository.GetBySectionIdAsync(sectionId, cancellationToken);
        if (_cacheService != null)
            await _cacheService.SetAsync(cacheKey, featureKeys, DefaultCacheExpiration, cancellationToken);

        return featureKeys;
    }

    public async Task<FeatureKeyDto> UpsertFeatureKeyAsync(FeatureKeyDto featureKey, CancellationToken cancellationToken = default)
    {
        FeatureKeyDto result;
        if (featureKey.Id == Guid.Empty)
        {
            result = await _featureKeyRepository.AddAsync(featureKey, cancellationToken);
        }
        else
        {
            await _featureKeyRepository.UpdateAsync(featureKey, cancellationToken);
            result = featureKey;
        }

        if (_cacheService != null)
        {
            var cacheKey = GetFeatureKeyCacheKey(result.Id);
            await _cacheService.SetAsync(cacheKey, result, DefaultCacheExpiration, cancellationToken);
            await _cacheService.RemoveAsync(GetFeatureKeysBySectionCacheKey(result.SectionId), cancellationToken);
        }

        return result;
    }

    public async Task DeleteFeatureKeyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var featureKey = await _featureKeyRepository.GetByIdAsync(id, cancellationToken);
        if (featureKey == null)
            return;

        await _featureKeyRepository.DeleteAsync(id, cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(GetFeatureKeyCacheKey(id), cancellationToken);
            await _cacheService.RemoveAsync(GetFeatureKeysBySectionCacheKey(featureKey.SectionId), cancellationToken);
        }
    }
}