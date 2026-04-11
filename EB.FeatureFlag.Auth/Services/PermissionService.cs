using EB.FeatureFlag.Auth.Abstractions.Permissions;

namespace EB.FeatureFlag.Auth.Services;

public class PermissionService : IPermissionService
{
    private static readonly Dictionary<Role, Permission> RolePermissions = new()
    {
        [Role.Viewer] = Permission.AllRead,
        [Role.Editor] = Permission.AllRead | Permission.FeatureFlagWrite,
        [Role.Manager] = Permission.AllRead | Permission.AllWrite | Permission.AllDelete | Permission.EnvironmentRotateKeys,
        [Role.Admin] = Permission.All
    };

    public Permission GetPermissions(IEnumerable<Role> roles)
    {
        return roles.Aggregate(Permission.None, (current, role) =>
            current | RolePermissions.GetValueOrDefault(role, Permission.None));
    }

    public bool HasPermission(IEnumerable<Role> roles, Permission requiredPermission)
    {
        var effective = GetPermissions(roles);
        return (effective & requiredPermission) == requiredPermission;
    }
}
