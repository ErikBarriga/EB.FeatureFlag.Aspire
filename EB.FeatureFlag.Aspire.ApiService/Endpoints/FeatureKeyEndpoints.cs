using EB.FeatureFlag.Aspire.ApiService.Models;
using EB.FeatureFlag.Data.IProvider;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Aspire.ApiService.Endpoints;

public static class FeatureKeyEndpoints
{
    public static IEndpointRouteBuilder MapFeatureKeyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("FeatureKeys");

        group.MapGet("/sections/{sectionId:guid}/feature-keys", async (Guid sectionId, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var featureKeys = await provider.GetFeatureKeysBySectionIdAsync(sectionId, ct);
            return Results.Ok(featureKeys);
        })
        .WithName("GetFeatureKeysBySection");

        group.MapGet("/feature-keys/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var featureKey = await provider.GetFeatureKeyByIdAsync(id, ct);
            return featureKey is not null ? Results.Ok(featureKey) : Results.NotFound();
        })
        .WithName("GetFeatureKeyById");

        group.MapPost("/sections/{sectionId:guid}/feature-keys", async (Guid sectionId, CreateFeatureKeyRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var dto = new FeatureKeyDto
            {
                SectionId = sectionId,
                Name = request.Name,
                Description = request.Description,
                Tags = request.Tags,
                Type = request.Type,
                Value = request.Value,
                ValidationRegex = request.ValidationRegex,
                ExternalConfig = request.ExternalConfig
            };

            try
            {
                var created = await provider.UpsertFeatureKeyAsync(dto, ct);
                return Results.Created($"/api/feature-keys/{created.Id}", created);
            }
            catch (FeatureKeyValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateFeatureKey");

        group.MapPut("/feature-keys/{id:guid}", async (Guid id, UpdateFeatureKeyRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetFeatureKeyByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.Tags = request.Tags;
            existing.Type = request.Type;
            existing.Value = request.Value;
            existing.ValidationRegex = request.ValidationRegex;
            existing.ExternalConfig = request.ExternalConfig;

            try
            {
                var updated = await provider.UpsertFeatureKeyAsync(existing, ct);
                return Results.Ok(updated);
            }
            catch (FeatureKeyValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("UpdateFeatureKey");

        group.MapDelete("/feature-keys/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetFeatureKeyByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            await provider.DeleteFeatureKeyAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteFeatureKey");

        return app;
    }
}
