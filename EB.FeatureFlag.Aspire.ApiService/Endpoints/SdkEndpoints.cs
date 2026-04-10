using EB.FeatureFlag.Aspire.ApiService.Models;
using EB.FeatureFlag.Data.IProvider;

namespace EB.FeatureFlag.Aspire.ApiService.Endpoints;

public static class SdkEndpoints
{
    public static IEndpointRouteBuilder MapSdkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sdk")
            .WithTags("SDK");

        group.MapGet("/feature-flags/{key}", async (
            string key,
            HttpContext httpContext,
            IFeatureFlagProvider provider,
            CancellationToken ct) =>
        {
            var envKey = httpContext.Request.Headers["X-Environment-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(envKey))
                return Results.Problem("Header 'X-Environment-Key' is required.", statusCode: 400);

            var result = await provider.GetFeatureFlagByKeyAndAccessKeyAsync(envKey, key, ct);
            if (result is null)
                return Results.NotFound(new { error = $"Feature flag '{key}' not found or invalid access key." });

            var (product, environment, section, flag) = result.Value;

            return Results.Ok(new SdkFeatureFlagResponse(
                Product: product,
                Environment: environment,
                Section: section,
                Key: flag.Key,
                Type: flag.Type,
                Value: flag.Value
            ));
        })
        .WithName("GetFeatureFlagByKey")
        .Produces<SdkFeatureFlagResponse>(200)
        .Produces(400)
        .Produces(404);

        group.MapGet("/feature-flags/{key}/value", async (
            string key,
            HttpContext httpContext,
            IFeatureFlagProvider provider,
            CancellationToken ct) =>
        {
            var envKey = httpContext.Request.Headers["X-Environment-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(envKey))
                return Results.Problem("Header 'X-Environment-Key' is required.", statusCode: 400);

            var result = await provider.GetFeatureFlagValueByKeyAndAccessKeyAsync(envKey, key, ct);
            if (result is null)
                return Results.NotFound(new { error = $"Feature flag '{key}' not found or invalid access key." });

            return Results.Ok(new SdkFeatureFlagValueResponse(
                Type: result.Type,
                Value: result.Value
            ));
        })
        .WithName("GetFeatureFlagValueByKey")
        .Produces<SdkFeatureFlagValueResponse>(200)
        .Produces(400)
        .Produces(404);

        return app;
    }
}
