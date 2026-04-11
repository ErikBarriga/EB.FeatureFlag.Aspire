using EB.FeatureFlag.Auth.Abstractions.Permissions;

namespace EB.FeatureFlag.Auth.Abstractions;

public interface IAuthUserService
{
    Task<IAuthUser> GetOrCreateUserAsync(AuthUserInfo externalInfo, CancellationToken cancellationToken = default);
    Task<IAuthUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IAuthUser?> GetUserByExternalIdAsync(string provider, string externalId, CancellationToken cancellationToken = default);
    Task<IAuthUser> UpdateUserRolesAsync(Guid userId, IEnumerable<Role> roles, CancellationToken cancellationToken = default);
    Task<IEnumerable<IAuthUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);
}
