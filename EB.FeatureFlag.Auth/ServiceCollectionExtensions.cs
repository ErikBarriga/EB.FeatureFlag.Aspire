using System.Text;
using EB.FeatureFlag.Auth.Abstractions;
using EB.FeatureFlag.Auth.Abstractions.Permissions;
using EB.FeatureFlag.Auth.Authorization;
using EB.FeatureFlag.Auth.Google;
using EB.FeatureFlag.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EB.FeatureFlag.Auth;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers auth for the API service (JWT bearer validation + permission-based authorization).
    /// </summary>
    public static IServiceCollection AddFeatureFlagApiAuth(
        this IServiceCollection services,
        Action<FeatureFlagAuthOptions> configure)
    {
        var options = new FeatureFlagAuthOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddSingleton<IPermissionService, PermissionService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = options.JwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.JwtSigningKey)),
                    ValidateLifetime = true
                };
            });

        services.AddFeatureFlagAuthorization();

        return services;
    }

    /// <summary>
    /// Registers auth for the Blazor Web app (OAuth login flow + cookie session + JWT generation for API calls).
    /// </summary>
    public static IServiceCollection AddFeatureFlagWebAuth(
        this IServiceCollection services,
        Action<FeatureFlagAuthOptions> configure)
    {
        var options = new FeatureFlagAuthOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddSingleton<IPermissionService, PermissionService>();
        services.AddSingleton<IAuthUserService, InMemoryAuthUserService>();
        services.AddSingleton<JwtTokenService>();

        AddAuthProvider(services, options);

        var authBuilder = services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultScheme = "Cookies";
            authOptions.DefaultChallengeScheme = GetChallengeScheme(options.ProviderType);
        })
        .AddCookie("Cookies");

        ConfigureOAuthProvider(authBuilder, options);

        services.AddFeatureFlagAuthorization();

        return services;
    }

    private static void AddAuthProvider(IServiceCollection services, FeatureFlagAuthOptions options)
    {
        switch (options.ProviderType)
        {
            case FeatureFlagAuthProviderType.Google:
                services.AddSingleton<IAuthProvider, GoogleAuthProvider>();
                break;
            default:
                throw new NotSupportedException($"Auth provider '{options.ProviderType}' is not supported.");
        }
    }

    private static void ConfigureOAuthProvider(Microsoft.AspNetCore.Authentication.AuthenticationBuilder builder, FeatureFlagAuthOptions options)
    {
        switch (options.ProviderType)
        {
            case FeatureFlagAuthProviderType.Google:
                builder.AddFeatureFlagGoogle(options.ClientId, options.ClientSecret);
                break;
            default:
                throw new NotSupportedException($"Auth provider '{options.ProviderType}' is not supported.");
        }
    }

    private static string GetChallengeScheme(FeatureFlagAuthProviderType providerType) => providerType switch
    {
        FeatureFlagAuthProviderType.Google => "Google",
        _ => throw new NotSupportedException($"Auth provider '{providerType}' is not supported.")
    };

    private static IServiceCollection AddFeatureFlagAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy("ProductRead", p => p.AddRequirements(new PermissionRequirement(Permission.ProductRead)))
            .AddPolicy("ProductWrite", p => p.AddRequirements(new PermissionRequirement(Permission.ProductWrite)))
            .AddPolicy("ProductDelete", p => p.AddRequirements(new PermissionRequirement(Permission.ProductDelete)))
            .AddPolicy("EnvironmentRead", p => p.AddRequirements(new PermissionRequirement(Permission.EnvironmentRead)))
            .AddPolicy("EnvironmentWrite", p => p.AddRequirements(new PermissionRequirement(Permission.EnvironmentWrite)))
            .AddPolicy("EnvironmentDelete", p => p.AddRequirements(new PermissionRequirement(Permission.EnvironmentDelete)))
            .AddPolicy("EnvironmentRotateKeys", p => p.AddRequirements(new PermissionRequirement(Permission.EnvironmentRotateKeys)))
            .AddPolicy("SectionRead", p => p.AddRequirements(new PermissionRequirement(Permission.SectionRead)))
            .AddPolicy("SectionWrite", p => p.AddRequirements(new PermissionRequirement(Permission.SectionWrite)))
            .AddPolicy("SectionDelete", p => p.AddRequirements(new PermissionRequirement(Permission.SectionDelete)))
            .AddPolicy("FeatureFlagRead", p => p.AddRequirements(new PermissionRequirement(Permission.FeatureFlagRead)))
            .AddPolicy("FeatureFlagWrite", p => p.AddRequirements(new PermissionRequirement(Permission.FeatureFlagWrite)))
            .AddPolicy("FeatureFlagDelete", p => p.AddRequirements(new PermissionRequirement(Permission.FeatureFlagDelete)))
            .AddPolicy("UserManagement", p => p.AddRequirements(new PermissionRequirement(Permission.UserManagement)));

        return services;
    }
}
