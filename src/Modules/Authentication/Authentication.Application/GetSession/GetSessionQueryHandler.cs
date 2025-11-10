using Authentication.Application.Abstractions.Services;
using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Entities;
using Core.Domain.Messaging;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Application.GetSession;

internal sealed class GetSessionQueryHandler(
    IAuthenticationService authenticationService,
    UserManager<User> userManager,
    IPermissionService permissionService) : IQueryHandler<GetSessionQuery, SessionResponse>
{
    private readonly IAuthenticationService m_authenticationService = authenticationService;
    private readonly UserManager<User> m_userManager = userManager;
    private readonly IPermissionService m_permissionService = permissionService;

    public async Task<Result<SessionResponse>> HandleAsync(GetSessionQuery query, CancellationToken cancellationToken)
    {
        User? currentUser = await m_authenticationService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            SessionResponse unauthenticatedResponse = new SessionResponse(
                IsAuthenticated: false,
                Username: null,
                UserId: null,
                Roles: Array.Empty<string>());

            return Result.Success(unauthenticatedResponse);
        }

        IList<string> rolesList = await m_userManager.GetRolesAsync(currentUser);
        IReadOnlyCollection<string> roles = rolesList.ToList().AsReadOnly();

        SessionResponse authenticatedResponse = new SessionResponse(
            IsAuthenticated: true,
            Username: currentUser.UserName,
            UserId: currentUser.Id,
            Roles: roles);

        return Result.Success(authenticatedResponse);
    }
}
