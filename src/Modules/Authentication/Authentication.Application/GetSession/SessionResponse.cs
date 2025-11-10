namespace Authentication.Application.GetSession;

public sealed record SessionResponse(
    bool IsAuthenticated,
    string? Username,
    string? UserId,
    IReadOnlyCollection<string> Roles);
