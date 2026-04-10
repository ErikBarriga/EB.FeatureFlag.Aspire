using System.Text.RegularExpressions;
using EB.FeatureFlag.Aspire.ApiService.Models;
using EB.FeatureFlag.Data.IProvider;
using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Aspire.ApiService.Endpoints;

public static partial class FeatureFlagEndpoints
{
    [GeneratedRegex(@"^[a-zA-Z0-9_\-\.\&\$]+$")]
    private static partial Regex FeatureFlagKeyRegex();

    private static bool IsValidFlagKey(string key) => FeatureFlagKeyRegex().IsMatch(key);

    public static IEndpointRouteBuilder MapFeatureFlagEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("FeatureFlags");

        // Get all feature flags by section
        group.MapGet("/sections/{sectionId:guid}/feature-flags", async (Guid sectionId, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var flags = await provider.GetFeatureFlagsBySectionIdAsync(sectionId, ct);
            return Results.Ok(flags);
        })
        .WithName("GetFeatureFlagsBySection");

        // Get all feature flags by product
        group.MapGet("/products/{productId:guid}/feature-flags", async (Guid productId, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var flags = await provider.GetFeatureFlagsByProductIdAsync(productId, ct);
            return Results.Ok(flags);
        })
        .WithName("GetFeatureFlagsByProduct");

        // Get feature flag by ID
        group.MapGet("/feature-flags/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var flag = await provider.GetFeatureFlagByIdAsync(id, ct);
            return flag is not null ? Results.Ok(flag) : Results.NotFound();
        })
        .WithName("GetFeatureFlagById");

        // Create feature flag under a section (auto-creates details for each environment)
        group.MapPost("/sections/{sectionId:guid}/feature-flags", async (Guid sectionId, CreateFeatureFlagRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            if (!IsValidFlagKey(request.Key))
                return Results.BadRequest(new { error = "Feature flag key must contain only alphanumeric characters and _ - . & $ (no spaces)." });

            var dto = new FeatureFlagDto
            {
                SectionId = sectionId,
                Key = request.Key,
                Description = request.Description,
                Tags = request.Tags,
                Type = request.Type,
                ValidationRegex = request.ValidationRegex
            };

            try
            {
                var created = await provider.UpsertFeatureFlagAsync(dto, ct);
                return Results.Created($"/api/feature-flags/{created.Id}", created);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateFeatureFlag");

        // Update feature flag
        group.MapPut("/feature-flags/{id:guid}", async (Guid id, UpdateFeatureFlagRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            if (!IsValidFlagKey(request.Key))
                return Results.BadRequest(new { error = "Feature flag key must contain only alphanumeric characters and _ - . & $ (no spaces)." });

            var existing = await provider.GetFeatureFlagByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            existing.Key = request.Key;
            existing.Description = request.Description;
            existing.Tags = request.Tags;
            existing.ValidationRegex = request.ValidationRegex;

            var updated = await provider.UpsertFeatureFlagAsync(existing, ct);
            return Results.Ok(updated);
        })
        .WithName("UpdateFeatureFlag");

        // Delete feature flag (cascades to details)
        group.MapDelete("/feature-flags/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetFeatureFlagByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            await provider.DeleteFeatureFlagAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteFeatureFlag");

        // --- FeatureFlagDetail (per-environment values) ---

        // Get details for a feature flag (one per environment)
        group.MapGet("/feature-flags/{featureFlagId:guid}/details", async (Guid featureFlagId, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var details = await provider.GetFeatureFlagDetailsByFlagIdAsync(featureFlagId, ct);
            return Results.Ok(details);
        })
        .WithName("GetFeatureFlagDetails");

        // Get single detail by ID
        group.MapGet("/feature-flag-details/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var detail = await provider.GetFeatureFlagDetailByIdAsync(id, ct);
            return detail is not null ? Results.Ok(detail) : Results.NotFound();
        })
        .WithName("GetFeatureFlagDetailById");

        // Update a detail (set value / external config for a specific environment)
        group.MapPut("/feature-flag-details/{id:guid}", async (Guid id, UpdateFeatureFlagDetailRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetFeatureFlagDetailByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            var flag = await provider.GetFeatureFlagByIdAsync(existing.FeatureFlagId, ct);
            if (flag is null)
                return Results.NotFound();

            existing.Value = request.Value;
            existing.ExternalConfig = request.ExternalConfig;

            try
            {
                var updated = await provider.UpsertFeatureFlagDetailAsync(existing, flag, ct);
                return Results.Ok(updated);
            }
            catch (Exception ex) when (ex is EB.FeatureFlag.Data.IProvider.Validation.FeatureKeyValidationException)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("UpdateFeatureFlagDetail");

        return app;
    }
}
