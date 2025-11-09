using Application.Abstractions.Messaging.Queries;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Messaging;

namespace Application.Authentication.GetSession;

internal sealed class GetSessionQueryHandler(
    IAuthenticationService authenticationService,
    IUserRepository userRepository,
    IPermissionService permissionService) : IQueryHandler<GetSessionQuery, SessionResponse>
{
    private readonly IAuthenticationService m_authenticationService = authenticationService;
    private readonly IUserRepository m_userRepository = userRepository;
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

        IReadOnlyCollection<string> roles = await m_userRepository.GetUserRolesAsync(currentUser.Id, cancellationToken);

        SessionResponse authenticatedResponse = new SessionResponse(
            IsAuthenticated: true,
            Username: currentUser.UserName,
            UserId: currentUser.Id,
            Roles: roles);

        return Result.Success(authenticatedResponse);
    }
}
