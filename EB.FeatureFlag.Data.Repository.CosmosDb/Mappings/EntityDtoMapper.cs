using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.Repository.CosmosDb.Entities;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Mappings;

public static class EntityDtoMapper
{
    // Product
    public static ProductDto ToDto(this ProductEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Tags = entity.Tags
    };

    public static ProductEntity ToEntity(this ProductDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        Tags = dto.Tags
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
        ProductId = entity.ProductId,
        Name = entity.Name,
        Description = entity.Description,
        Tags = entity.Tags
    };

    public static SectionEntity ToEntity(this SectionDto dto) => new()
    {
        Id = dto.Id,
        ProductId = dto.ProductId,
        Name = dto.Name,
        Description = dto.Description,
        Tags = dto.Tags
    };

    // FeatureFlag
    public static FeatureFlagDto ToDto(this FeatureFlagEntity entity) => new()
    {
        Id = entity.Id,
        ProductId = entity.ProductId,
        SectionId = entity.SectionId,
        Key = entity.Key,
        Description = entity.Description,
        Tags = entity.Tags,
        Type = entity.Type,
        ValidationRegex = entity.ValidationRegex
    };

    public static FeatureFlagEntity ToEntity(this FeatureFlagDto dto) => new()
    {
        Id = dto.Id,
        ProductId = dto.ProductId,
        SectionId = dto.SectionId,
        Key = dto.Key,
        Description = dto.Description,
        Tags = dto.Tags,
        Type = dto.Type,
        ValidationRegex = dto.ValidationRegex
    };

    // FeatureFlagDetail
    public static FeatureFlagDetailDto ToDto(this FeatureFlagDetailEntity entity) => new()
    {
        Id = entity.Id,
        FeatureFlagId = entity.FeatureFlagId,
        EnvironmentId = entity.EnvironmentId,
        ProductId = entity.ProductId,
        Value = entity.Value,
        ExternalConfig = entity.ExternalConfig
    };

    public static FeatureFlagDetailEntity ToEntity(this FeatureFlagDetailDto dto) => new()
    {
        Id = dto.Id,
        FeatureFlagId = dto.FeatureFlagId,
        EnvironmentId = dto.EnvironmentId,
        ProductId = dto.ProductId,
        Value = dto.Value,
        ExternalConfig = dto.ExternalConfig
    };
}