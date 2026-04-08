using EB.FeatureFlag.Aspire.ApiService.Models;
using EB.FeatureFlag.Data.IProvider;
using EB.FeatureFlag.Data.IRepository.DTOs;

namespace EB.FeatureFlag.Aspire.ApiService.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", async (IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var products = await provider.GetAllProductsAsync(ct);
            return Results.Ok(products);
        })
        .WithName("GetAllProducts");

        group.MapGet("/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var product = await provider.GetProductByIdAsync(id, ct);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById");

        group.MapPost("/", async (CreateProductRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var dto = new ProductDto
            {
                Name = request.Name,
                Description = request.Description,
                Tags = request.Tags
            };

            var created = await provider.UpsertProductAsync(dto, ct);
            return Results.Created($"/api/products/{created.Id}", created);
        })
        .WithName("CreateProduct");

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetProductByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.Tags = request.Tags;

            var updated = await provider.UpsertProductAsync(existing, ct);
            return Results.Ok(updated);
        })
        .WithName("UpdateProduct");

        group.MapDelete("/{id:guid}", async (Guid id, IFeatureFlagProvider provider, CancellationToken ct) =>
        {
            var existing = await provider.GetProductByIdAsync(id, ct);
            if (existing is null)
                return Results.NotFound();

            await provider.DeleteProductAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteProduct");

        return app;
    }
}
