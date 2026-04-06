using EB.FeatureFlag.Aspire.ApiService.Models;
using EB.FeatureFlag.Data.IProvider;
using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Aspire.ApiService.Endpoints;

public static class EnvironmentEndpoints
{
    public static IEndpointRouteBuilder MapEnvironmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Environments");

        group.MapGet("/products/{productId:guid}/environments", async (Guid productId, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var environments = await provider.GetEnvironmentsByProductIdAsync(productId, ct);
            return Results.Ok(environments);
        })
        .WithName("GetEnvironmentsByProduct");

        group.MapGet("/environments/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var environment = await provider.GetEnvironmentByIdAsync(id, ct);
            return environment is not null ? Results.Ok(environment) : Results.NotFound();
        })
        .WithName("GetEnvironmentById");

        group.MapPost("/products/{productId:guid}/environments", async (Guid productId, CreateEnvironmentRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var dto = new EnvironmentDto
            {
                ProductId = productId,
                Name = request.Name,
                Description = request.Description,
                Tags = request.Tags
            };

            var created = await provider.UpsertEnvironmentAsync(dto, ct);
            return Results.Created($"/api/environments/{created.Id}", created);
        })
        .WithName("CreateEnvironment");

        group.MapPut("/environments/{id:guid}", async (Guid id, UpdateEnvironmentRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetEnvironmentByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.Tags = request.Tags;

            var updated = await provider.UpsertEnvironmentAsync(existing, ct);
            return Results.Ok(updated);
        })
        .WithName("UpdateEnvironment");

        group.MapDelete("/environments/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetEnvironmentByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            await provider.DeleteEnvironmentAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteEnvironment");

        group.MapPost("/environments/{id:guid}/rotate-keys", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            try
            {
                var rotated = await provider.RotateEnvironmentKeysAsync(id, ct);
                return Results.Ok(rotated);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithName("RotateEnvironmentKeys");

        return app;
    }
}
