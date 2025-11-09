namespace Application.Authentication.GetSession;

public sealed record SessionResponse(
    bool IsAuthenticated,
    string? Username,
    string? UserId,
    IReadOnlyCollection<string> Roles);
