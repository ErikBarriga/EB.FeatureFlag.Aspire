using EB.FeatureFlag.Aspire.ApiService.Models;
using EB.FeatureFlag.Data.IProvider;

namespace EB.FeatureFlag.Aspire.ApiService.Endpoints;

public static class SdkEndpoints
{
    public static IEndpointRouteBuilder MapSdkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sdk")
            .WithTags("SDK");

        group.MapGet("/feature-flags/{name}", async (
            string name,
            HttpContext httpContext,
            IFeatureFlagProvider provider,
            CancellationToken ct) =>
        {
            var envKey = httpContext.Request.Headers["X-Environment-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(envKey))
                return Results.Problem("Header 'X-Environment-Key' is required.", statusCode: 400);

            var result = await provider.GetFeatureFlagByNameAndAccessKeyAsync(envKey, name, ct);
            if (result is null)
                return Results.NotFound(new { error = $"Feature flag '{name}' not found or invalid access key." });

            var (product, environment, section, flag) = result.Value;

            return Results.Ok(new SdkSingleFeatureFlagResponse(
                Product: product,
                Environment: environment,
                Section: section,
                Name: flag.Name,
                Type: flag.Type,
                Value: flag.Value
            ));
        })
        .WithName("GetFeatureFlagByName")
        .Produces<SdkSingleFeatureFlagResponse>(200)
        .Produces(400)
        .Produces(404);

        return app;
    }
}
