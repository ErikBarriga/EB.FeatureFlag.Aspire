using EB.FeatureFlag.Auth.Abstractions;
using EB.FeatureFlag.Auth.Abstractions.Permissions;

namespace EB.FeatureFlag.Auth.Services;

public class AuthUser : IAuthUser
{
    public Guid UserId { get; init; }
    public string ExternalId { get; init; } = default!;
    public string Provider { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? PictureUrl { get; init; }
    public IReadOnlyList<Role> Roles { get; set; } = [Role.Viewer];
}
