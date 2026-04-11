using System.Collections.Concurrent;
using EB.FeatureFlag.Auth.Abstractions;
using EB.FeatureFlag.Auth.Abstractions.Permissions;

namespace EB.FeatureFlag.Auth.Services;

public class InMemoryAuthUserService : IAuthUserService
{
    private readonly ConcurrentDictionary<Guid, AuthUser> _usersById = new();
    private readonly ConcurrentDictionary<string, Guid> _externalIndex = new();
    private readonly Role _defaultRole;

    public InMemoryAuthUserService(FeatureFlagAuthOptions options)
    {
        _defaultRole = options.DefaultRole;
    }

    public Task<IAuthUser> GetOrCreateUserAsync(AuthUserInfo externalInfo, CancellationToken cancellationToken = default)
    {
        var indexKey = $"{externalInfo.Provider}:{externalInfo.ExternalId}";

        if (_externalIndex.TryGetValue(indexKey, out var existingId) && _usersById.TryGetValue(existingId, out var existing))
        {
            return Task.FromResult<IAuthUser>(existing);
        }

        var user = new AuthUser
        {
            UserId = Guid.NewGuid(),
            ExternalId = externalInfo.ExternalId,
            Provider = externalInfo.Provider,
            DisplayName = externalInfo.DisplayName,
            Email = externalInfo.Email,
            PictureUrl = externalInfo.PictureUrl,
            Roles = [_defaultRole]
        };

        _usersById[user.UserId] = user;
        _externalIndex[indexKey] = user.UserId;

        return Task.FromResult<IAuthUser>(user);
    }

    public Task<IAuthUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _usersById.TryGetValue(userId, out var user);
        return Task.FromResult<IAuthUser?>(user);
    }

    public Task<IAuthUser?> GetUserByExternalIdAsync(string provider, string externalId, CancellationToken cancellationToken = default)
    {
        var indexKey = $"{provider}:{externalId}";
        if (_externalIndex.TryGetValue(indexKey, out var userId) && _usersById.TryGetValue(userId, out var user))
            return Task.FromResult<IAuthUser?>(user);
        return Task.FromResult<IAuthUser?>(null);
    }

    public Task<IAuthUser> UpdateUserRolesAsync(Guid userId, IEnumerable<Role> roles, CancellationToken cancellationToken = default)
    {
        if (!_usersById.TryGetValue(userId, out var user))
            throw new KeyNotFoundException($"User '{userId}' not found.");

        user.Roles = roles.ToList().AsReadOnly();
        return Task.FromResult<IAuthUser>(user);
    }

    public Task<IEnumerable<IAuthUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<IAuthUser>>(_usersById.Values.ToList());
    }
}
