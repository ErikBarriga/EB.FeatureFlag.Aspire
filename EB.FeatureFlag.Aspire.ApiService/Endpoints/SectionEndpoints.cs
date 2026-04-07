using EB.FeatureFlag.Aspire.ApiService.Models;
using EB.FeatureFlag.Data.IProvider;
using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Aspire.ApiService.Endpoints;

public static class SectionEndpoints
{
    public static IEndpointRouteBuilder MapSectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Sections");

        group.MapGet("/products/{productId:guid}/sections", async (Guid productId, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var sections = await provider.GetSectionsByProductIdAsync(productId, ct);
            return Results.Ok(sections);
        })
        .WithName("GetSectionsByProduct");

        group.MapGet("/sections/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var section = await provider.GetSectionByIdAsync(id, ct);
            return section is not null ? Results.Ok(section) : Results.NotFound();
        })
        .WithName("GetSectionById");

        group.MapPost("/products/{productId:guid}/sections", async (Guid productId, CreateSectionRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var dto = new SectionDto
            {
                ProductId = productId,
                Name = request.Name,
                Description = request.Description,
                Tags = request.Tags
            };

            try
            {
                var created = await provider.UpsertSectionAsync(dto, ct);
                return Results.Created($"/api/sections/{created.Id}", created);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateSection");

        group.MapPut("/sections/{id:guid}", async (Guid id, UpdateSectionRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetSectionByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.Tags = request.Tags;

            var updated = await provider.UpsertSectionAsync(existing, ct);
            return Results.Ok(updated);
        })
        .WithName("UpdateSection");

        group.MapDelete("/sections/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetSectionByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            await provider.DeleteSectionAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteSection");

        return app;
    }
}
