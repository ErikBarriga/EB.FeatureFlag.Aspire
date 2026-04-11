using System.Security.Claims;
using EB.FeatureFlag.Auth.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace EB.FeatureFlag.Auth.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var roleClaims = context.User.FindAll(ClaimTypes.Role);
        var roles = roleClaims
            .Select(c => Enum.TryParse<Role>(c.Value, out var role) ? role : (Role?)null)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .ToList();

        if (_permissionService.HasPermission(roles, requirement.RequiredPermission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
