using EB.FeatureFlag.Auth.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace EB.FeatureFlag.Auth.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public Permission RequiredPermission { get; }

    public PermissionRequirement(Permission permission)
    {
        RequiredPermission = permission;
    }
}
