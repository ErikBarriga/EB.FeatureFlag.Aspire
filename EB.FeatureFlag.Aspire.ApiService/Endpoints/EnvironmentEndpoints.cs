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
            return Results.Ok(environments.Select(ToResponse));
        })
        .WithName("GetEnvironmentsByProduct");

        group.MapGet("/environments/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var environment = await provider.GetEnvironmentByIdAsync(id, ct);
            return environment is not null ? Results.Ok(ToResponse(environment)) : Results.NotFound();
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
            var response = new EnvironmentCreatedResponse(
                created.Id, created.ProductId, created.Name, created.Description, created.Tags,
                created.PrimaryAccessKey, created.SecondaryAccessKey);
            return Results.Created($"/api/environments/{created.Id}", response);
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
            return Results.Ok(ToResponse(updated));
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

        group.MapPost("/environments/{id:guid}/rotate-keys", async (Guid id, RotateKeyRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            if (request.KeyType is not ("Primary" or "Secondary"))
                return Results.BadRequest("KeyType must be 'Primary' or 'Secondary'.");

            try
            {
                var rotated = await provider.RotateEnvironmentKeysAsync(id, request.KeyType, ct);
                var newKey = request.KeyType == "Primary" ? rotated.PrimaryAccessKey : rotated.SecondaryAccessKey;
                return Results.Ok(new EnvironmentRotatedKeyResponse(rotated.Id, request.KeyType, newKey));
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithName("RotateEnvironmentKeys");

        return app;
    }

    private static EnvironmentResponse ToResponse(EnvironmentDto dto) =>
        new(dto.Id, dto.ProductId, dto.Name, dto.Description, dto.Tags);
}
