using System.Security.Claims;
using Users.Application.Abstractions.Repositories;
using Users.Application.Abstractions.Services;
using Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory m_serviceScopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        m_serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
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

        IUserRepository userRepository = scope.ServiceProvider
            .GetRequiredService<IUserRepository>();

        HashSet<string> permissions = await userRepository.GetUserPermissionsAsync(userId);

        if (permissions.Contains(nameof(PermissionType.FullAccess)) || permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
