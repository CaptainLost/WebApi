using Application.Abstractions.Messaging.Queries;
using Application.Abstractions.Services;
using Domain.Messaging;
using Domain.Users;

namespace Application.Authentication.GetSession;

internal sealed class GetSessionQueryHandler(
    IAuthenticationService authenticationService) : IQueryHandler<GetSessionQuery, SessionResponse>
{
    private readonly IAuthenticationService m_authenticationService = authenticationService;

    public async Task<Result<SessionResponse>> HandleAsync(GetSessionQuery query, CancellationToken cancellationToken)
    {
        User? currentUser = await m_authenticationService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            SessionResponse unauthenticatedResponse = new SessionResponse(
                IsAuthenticated: false,
                Username: null,
                UserId: null);

            return Result<SessionResponse>.Success(unauthenticatedResponse);
        }

        SessionResponse authenticatedResponse = new SessionResponse(
            IsAuthenticated: true,
            Username: currentUser.UserName,
            UserId: currentUser.Id);

        return Result<SessionResponse>.Success(authenticatedResponse);
    }
}
