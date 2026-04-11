using EB.FeatureFlag.Auth.Abstractions.Permissions;

namespace EB.FeatureFlag.Auth;

public class FeatureFlagAuthOptions
{
    public FeatureFlagAuthProviderType ProviderType { get; set; } = FeatureFlagAuthProviderType.Google;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string JwtSigningKey { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = "EB.FeatureFlag";
    public string JwtAudience { get; set; } = "EB.FeatureFlag.Api";
    public int JwtTokenLifetimeMinutes { get; set; } = 60;
    public Role DefaultRole { get; set; } = Role.Viewer;
}
