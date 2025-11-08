using System.Security.Claims;
using Application.Abstractions.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authentication.Authorization;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory m_serviceScopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        m_serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            return;
        }

        string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        using IServiceScope scope = m_serviceScopeFactory.CreateScope();

        IPermissionService permissionService = scope.ServiceProvider
            .GetRequiredService<IPermissionService>();

        HashSet<string> permissions = await permissionService.GetPermissionsAsync(userId);

        if (permissions.Contains(nameof(PermissionType.FullAccess)) || permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
