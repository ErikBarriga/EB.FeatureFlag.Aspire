using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace EB.FeatureFlag.Auth.Google;

public static class GoogleAuthenticationExtensions
{
    public static AuthenticationBuilder AddFeatureFlagGoogle(
        this AuthenticationBuilder builder,
        string clientId,
        string clientSecret)
    {
        builder.AddGoogle(options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
            options.CallbackPath = "/signin-google";
            options.SaveTokens = true;
            options.Scope.Add("email");
            options.Scope.Add("profile");
        });

        return builder;
    }
}
