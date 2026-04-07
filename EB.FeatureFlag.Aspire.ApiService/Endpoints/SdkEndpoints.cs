using EB.FeatureFlag.Aspire.ApiService.Models;
using EB.FeatureFlag.Data.IProvider;

namespace EB.FeatureFlag.Aspire.ApiService.Endpoints;

public static class SdkEndpoints
{
    public static IEndpointRouteBuilder MapSdkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sdk")
            .WithTags("SDK");

        group.MapGet("/feature-flags", async (
            HttpContext httpContext,
            IFeatureFlagProvider provider,
            CancellationToken ct) =>
        {
            var productKey = httpContext.Request.Headers["X-Product-Key"].FirstOrDefault();
            var envKey = httpContext.Request.Headers["X-Environment-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(productKey) || string.IsNullOrWhiteSpace(envKey))
                return Results.Problem("Headers 'X-Product-Key' and 'X-Environment-Key' are required.", statusCode: 400);

            var result = await provider.GetFeatureFlagsByAccessKeysAsync(productKey, envKey, ct);
            if (result == null)
                return Results.Problem("Invalid access keys.", statusCode: 401);

            var (product, environment, sections) = result.Value;

            var response = new SdkFeatureFlagsResponse(
                Product: product.Name,
                Environment: environment.Name,
                Sections: sections.Select(s => new SdkSectionResponse(
                    Name: s.SectionName,
                    Keys: s.FeatureKeys.Select(fk => new SdkFeatureKeyResponse(
                        Name: fk.Name,
                        Type: fk.Type,
                        Value: fk.Value
                    )).ToList()
                )).ToList()
            );

            return Results.Ok(response);
        })
        .WithName("GetFeatureFlags")
        .Produces<SdkFeatureFlagsResponse>(200)
        .Produces(400)
        .Produces(401);

        return app;
    }
}
