using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.Repository.SQLite.Entities;

namespace EB.FeatureFlag.Data.Repository.SQLite.Mappings;

public static class EntityDtoMapper
{
    // Product
    public static ProductDto ToDto(this ProductEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Tags = entity.Tags,
        PrimaryAccessKey = entity.PrimaryAccessKey,
        SecondaryAccessKey = entity.SecondaryAccessKey
    };

    public static ProductEntity ToEntity(this ProductDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        Tags = dto.Tags,
        PrimaryAccessKey = dto.PrimaryAccessKey,
        SecondaryAccessKey = dto.SecondaryAccessKey
    };

    // Environment
    public static EnvironmentDto ToDto(this EnvironmentEntity entity) => new()
    {
        Id = entity.Id,
        ProductId = entity.ProductId,
        Name = entity.Name,
        Description = entity.Description,
        Tags = entity.Tags,
        PrimaryAccessKey = entity.PrimaryAccessKey,
        SecondaryAccessKey = entity.SecondaryAccessKey
    };

    public static EnvironmentEntity ToEntity(this EnvironmentDto dto) => new()
    {
        Id = dto.Id,
        ProductId = dto.ProductId,
        Name = dto.Name,
        Description = dto.Description,
        Tags = dto.Tags,
        PrimaryAccessKey = dto.PrimaryAccessKey,
        SecondaryAccessKey = dto.SecondaryAccessKey
    };

    // Section
    public static SectionDto ToDto(this SectionEntity entity) => new()
    {
        Id = entity.Id,
        EnvironmentId = entity.EnvironmentId,
        Name = entity.Name,
        Description = entity.Description,
        Tags = entity.Tags
    };

    public static SectionEntity ToEntity(this SectionDto dto, Guid productId) => new()
    {
        Id = dto.Id,
        EnvironmentId = dto.EnvironmentId,
        ProductId = productId,
        Name = dto.Name,
        Description = dto.Description,
        Tags = dto.Tags
    };

    // ExternalSourceConfig
    public static ExternalSourceConfigDto ToDto(this ExternalSourceConfigEntity entity) => new()
    {
        Url = entity.Endpoint,
        Method = "GET",
        Headers = entity.Headers?.ToDictionary(h => h.Key, h => h.Value),
        Auth = null,
        MappingPath = null,
        Body = null,
        ContentType = null
    };

    public static ExternalSourceConfigEntity ToEntity(this ExternalSourceConfigDto dto, Guid featureKeyId) => new()
    {
        Id = Guid.NewGuid(),
        FeatureKeyId = featureKeyId,
        Source = "External",
        Endpoint = dto.Url,
        AuthToken = null,
        Headers = dto.Headers?.Select(kvp => new ExternalSourceHeaderEntity
        {
            Id = Guid.NewGuid(),
            Key = kvp.Key,
            Value = kvp.Value
        }).ToList() ?? []
    };

    // FeatureKey
    public static FeatureKeyDto ToDto(this FeatureKeyEntity entity) => new()
    {
        Id = entity.Id,
        SectionId = entity.SectionId,
        Name = entity.Name,
        Description = entity.Description,
        Tags = entity.Tags,
        Type = entity.Type,
        Value = entity.Value,
        ValidationRegex = entity.ValidationRegex,
        ExternalConfig = entity.ExternalConfig?.ToDto()
    };

    public static FeatureKeyEntity ToEntity(this FeatureKeyDto dto, Guid environmentId, Guid productId) => new()
    {
        Id = dto.Id,
        SectionId = dto.SectionId,
        EnvironmentId = environmentId,
        ProductId = productId,
        Name = dto.Name,
        Description = dto.Description,
        Tags = dto.Tags,
        Type = dto.Type,
        Value = dto.Value,
        ValidationRegex = dto.ValidationRegex,
        ExternalConfig = dto.ExternalConfig != null ? dto.ExternalConfig.ToEntity(dto.Id) : null
    };
}
