using EB.FeatureFlag.Data.ICache;
using EB.FeatureFlag.Data.IProvider;
using EB.FeatureFlag.Data.IProvider.ExternalSource;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Interfaces;

namespace EB.FeatureFlag.Data.Provider;

public class FeatureFlagProvider : IFeatureFlagProvider
{
    private readonly IProductRepository _productRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IFeatureFlagRepository _featureFlagRepository;
    private readonly IFeatureFlagDetailRepository _featureFlagDetailRepository;
    private readonly ICacheService? _cacheService;
    private readonly IFeatureKeyValueValidatorFactory? _validatorFactory;
    private readonly IExternalSourceService? _externalSourceService;
    private static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMinutes(10);

    public FeatureFlagProvider(
        IProductRepository productRepository,
        IEnvironmentRepository environmentRepository,
        ISectionRepository sectionRepository,
        IFeatureFlagRepository featureFlagRepository,
        IFeatureFlagDetailRepository featureFlagDetailRepository,
        ICacheService? cacheService = null,
        IFeatureKeyValueValidatorFactory? validatorFactory = null,
        IExternalSourceService? externalSourceService = null)
    {
        _productRepository = productRepository;
        _environmentRepository = environmentRepository;
        _sectionRepository = sectionRepository;
        _featureFlagRepository = featureFlagRepository;
        _featureFlagDetailRepository = featureFlagDetailRepository;
        _cacheService = cacheService;
        _validatorFactory = validatorFactory;
        _externalSourceService = externalSourceService;
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
    private static string GetSectionsByProductCacheKey(Guid productId) => $"FeatureFlag:Section:ByProduct:{productId}";

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

    public async Task<IEnumerable<SectionDto>> GetSectionsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetSectionsByProductCacheKey(productId);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<IEnumerable<SectionDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;
        }

        var sections = await _sectionRepository.GetByProductIdAsync(productId, cancellationToken);
        if (_cacheService != null)
            await _cacheService.SetAsync(cacheKey, sections, DefaultCacheExpiration, cancellationToken);

        return sections;
    }

    public async Task<SectionDto> UpsertSectionAsync(SectionDto section, CancellationToken cancellationToken = default)
    {
        SectionDto result;
        if (section.Id == Guid.Empty)
        {
            var product = await _productRepository.GetByIdAsync(section.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException($"Product '{section.ProductId}' not found.");

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
            await _cacheService.RemoveAsync(GetSectionsByProductCacheKey(result.ProductId), cancellationToken);
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
            await _cacheService.RemoveAsync(GetSectionsByProductCacheKey(section.ProductId), cancellationToken);
        }
    }

    // --- FeatureFlag (definition) ---
    public async Task<FeatureFlagDto?> GetFeatureFlagByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _featureFlagRepository.GetByIdAsync(id, cancellationToken);

    public async Task<IEnumerable<FeatureFlagDto>> GetFeatureFlagsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        => await _featureFlagRepository.GetBySectionIdAsync(sectionId, cancellationToken);

    public async Task<IEnumerable<FeatureFlagDto>> GetFeatureFlagsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await _featureFlagRepository.GetByProductIdAsync(productId, cancellationToken);

    public async Task<FeatureFlagDto> UpsertFeatureFlagAsync(FeatureFlagDto featureFlag, CancellationToken cancellationToken = default)
    {
        FeatureFlagDto result;
        if (featureFlag.Id == Guid.Empty)
        {
            var section = await _sectionRepository.GetByIdAsync(featureFlag.SectionId, cancellationToken)
                ?? throw new KeyNotFoundException($"Section '{featureFlag.SectionId}' not found.");

            featureFlag.ProductId = section.ProductId;
            result = await _featureFlagRepository.AddAsync(featureFlag, cancellationToken);

            // Auto-create a FeatureFlagDetail for each environment in the product
            var environments = await _environmentRepository.GetByProductIdAsync(result.ProductId, cancellationToken);
            foreach (var env in environments)
            {
                var detail = new FeatureFlagDetailDto
                {
                    FeatureFlagId = result.Id,
                    EnvironmentId = env.Id,
                    ProductId = result.ProductId
                };
                await _featureFlagDetailRepository.AddAsync(detail, cancellationToken);
            }
        }
        else
        {
            await _featureFlagRepository.UpdateAsync(featureFlag, cancellationToken);
            result = featureFlag;
        }

        return result;
    }

    public async Task DeleteFeatureFlagAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _featureFlagDetailRepository.DeleteByFeatureFlagIdAsync(id, cancellationToken);
        await _featureFlagRepository.DeleteAsync(id, cancellationToken);
    }

    // --- FeatureFlagDetail (per-environment value) ---
    public async Task<FeatureFlagDetailDto?> GetFeatureFlagDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var detail = await _featureFlagDetailRepository.GetByIdAsync(id, cancellationToken);
        if (detail == null) return null;

        var flag = await _featureFlagRepository.GetByIdAsync(detail.FeatureFlagId, cancellationToken);
        if (flag != null)
            detail = await ResolveExternalDetailValueAsync(detail, flag, cancellationToken);

        return detail;
    }

    public async Task<IEnumerable<FeatureFlagDetailDto>> GetFeatureFlagDetailsByFlagIdAsync(Guid featureFlagId, CancellationToken cancellationToken = default)
    {
        var details = (await _featureFlagDetailRepository.GetByFeatureFlagIdAsync(featureFlagId, cancellationToken)).ToList();
        var flag = await _featureFlagRepository.GetByIdAsync(featureFlagId, cancellationToken);

        if (flag != null)
        {
            for (var i = 0; i < details.Count; i++)
                details[i] = await ResolveExternalDetailValueAsync(details[i], flag, cancellationToken) ?? details[i];
        }

        return details;
    }

    public async Task<FeatureFlagDetailDto> UpsertFeatureFlagDetailAsync(FeatureFlagDetailDto detail, FeatureFlagDto featureFlag, CancellationToken cancellationToken = default)
    {
        if (detail.ExternalConfig == null || detail.Value != null)
            _validatorFactory?.Validate(featureFlag.Type, detail.Value, featureFlag.ValidationRegex);

        if (detail.Id == Guid.Empty)
        {
            detail.ProductId = featureFlag.ProductId;
            return await _featureFlagDetailRepository.AddAsync(detail, cancellationToken);
        }

        await _featureFlagDetailRepository.UpdateAsync(detail, cancellationToken);
        return detail;
    }

    // --- SDK / Public API ---
    private static string GetSdkCacheKey(Guid productId, Guid environmentId) => $"FeatureFlag:Sdk:{productId}:{environmentId}";
    private static readonly TimeSpan SdkCacheExpiration = TimeSpan.FromMinutes(5);

    public async Task<(ProductDto Product, EnvironmentDto Environment, IEnumerable<SdkSectionFlagsDto> Sections)?> GetFeatureFlagsByAccessKeyAsync(
        string environmentKey, CancellationToken cancellationToken = default)
    {
        var environment = await _environmentRepository.GetByAccessKeyAsync(environmentKey, cancellationToken);
        if (environment == null) return null;

        var product = await _productRepository.GetByIdAsync(environment.ProductId, cancellationToken);
        if (product == null) return null;

        var cacheKey = GetSdkCacheKey(product.Id, environment.Id);
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<List<SdkSectionFlagsDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return (product, environment, cached);
        }

        var sections = await _sectionRepository.GetByProductIdAsync(product.Id, cancellationToken);
        var result = new List<SdkSectionFlagsDto>();

        foreach (var section in sections)
        {
            var flags = await _featureFlagRepository.GetBySectionIdAsync(section.Id, cancellationToken);
            var sdkFlags = new List<SdkFeatureFlagItemDto>();

            foreach (var flag in flags)
            {
                var details = await _featureFlagDetailRepository.GetByFeatureFlagIdAsync(flag.Id, cancellationToken);
                var detail = details.FirstOrDefault(d => d.EnvironmentId == environment.Id);

                if (detail != null)
                    detail = await ResolveExternalDetailValueAsync(detail, flag, cancellationToken);

                sdkFlags.Add(new SdkFeatureFlagItemDto
                {
                    Name = flag.Name,
                    Type = flag.Type,
                    Value = detail?.Value
                });
            }

            result.Add(new SdkSectionFlagsDto
            {
                SectionName = section.Name,
                FeatureFlags = sdkFlags
            });
        }

        if (_cacheService != null)
            await _cacheService.SetAsync(cacheKey, result, SdkCacheExpiration, cancellationToken);

        return (product, environment, result);
    }

    private async Task<FeatureFlagDetailDto?> ResolveExternalDetailValueAsync(FeatureFlagDetailDto? detail, FeatureFlagDto flag, CancellationToken cancellationToken)
    {
        if (detail == null || detail.ExternalConfig == null || _externalSourceService == null)
            return detail;

        try
        {
            var fetchedValue = await _externalSourceService.FetchExternalValueAsync(detail.ExternalConfig, flag.Type, cancellationToken);
            if (fetchedValue != null)
            {
                try
                {
                    _validatorFactory?.Validate(flag.Type, fetchedValue, flag.ValidationRegex);
                    detail.Value = fetchedValue;
                }
                catch { }
            }
        }
        catch { }

        return detail;
    }
}

