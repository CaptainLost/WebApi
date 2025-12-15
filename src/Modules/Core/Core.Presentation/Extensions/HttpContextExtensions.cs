using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Core.Presentation.Extensions;

public static class HttpContextExtensions
{
    public static Guid? GetUserId(this HttpContext httpContext)
    {
        string? userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString))
        {
            return null;
        }

        return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
    }

    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        string? userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString))
        {
            return null;
        }

        return Guid.TryParse(userIdString, out Guid userId) ? userId : null;
    }
}
