namespace EB.FeatureFlag.Auth.Abstractions.Permissions;

public interface IPermissionService
{
    Permission GetPermissions(IEnumerable<Role> roles);
    bool HasPermission(IEnumerable<Role> roles, Permission requiredPermission);
}
