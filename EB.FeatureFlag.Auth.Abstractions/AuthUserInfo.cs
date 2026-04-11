namespace EB.FeatureFlag.Auth.Abstractions;

public class AuthUserInfo
{
    public required string ExternalId { get; init; }
    public required string Provider { get; init; }
    public required string Email { get; init; }
    public required string DisplayName { get; init; }
    public string? PictureUrl { get; init; }
}
