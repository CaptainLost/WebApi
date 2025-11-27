using Users.Application.Abstractions.Services;
using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Microsoft.AspNetCore.Identity;
using Users.Application.Abstractions.Repositories;
using Users.Domain.Entities;

namespace Users.Application.GetSession;

internal sealed class GetSessionQueryHandler(
    IAuthenticationService authenticationService,
    IUserRepository userRepository,
    UserManager<User> userManager) : IQueryHandler<GetSessionQuery, SessionResponse>
{
    private readonly IAuthenticationService m_authenticationService = authenticationService;
    private readonly IUserRepository m_userRepository = userRepository;

    private readonly UserManager<User> m_userManager = userManager;

    public async Task<Result<SessionResponse>> HandleAsync(GetSessionQuery query, CancellationToken cancellationToken)
    {
        string? currentUserId = await m_authenticationService.GetCurrentUserIdAsync();

        if (currentUserId == null)
        {
            SessionResponse unauthenticatedResponse = new SessionResponse(
                IsAuthenticated: false,
                Username: null,
                UserId: null,
                Roles: Array.Empty<string>());

            return Result.Success(unauthenticatedResponse);
        }

        User? currentUser = await m_userRepository.GetUserByIdWithRolesAsync(currentUserId);
        
        if (currentUser == null)
        {
            SessionResponse notFoundResponse = new SessionResponse(
                IsAuthenticated: false,
                Username: null,
                UserId: null,
                Roles: Array.Empty<string>());

            return Result.Success(notFoundResponse);
        }

        IReadOnlyCollection<string> roleNames = currentUser.Roles
            .Select(role => role.Name)
            .ToArray();

        SessionResponse authenticatedResponse = new SessionResponse(
            IsAuthenticated: true,
            Username: currentUser.UserName,
            UserId: currentUser.Id,
            Roles: roleNames);

        return Result.Success(authenticatedResponse);
    }
}
