using EB.FeatureFlag.Auth.Abstractions.Permissions;

namespace EB.FeatureFlag.Auth.Abstractions;

public interface IAuthUser
{
    Guid UserId { get; }
    string ExternalId { get; }
    string Provider { get; }
    string DisplayName { get; }
    string Email { get; }
    string? PictureUrl { get; }
    IReadOnlyList<Role> Roles { get; }
}
