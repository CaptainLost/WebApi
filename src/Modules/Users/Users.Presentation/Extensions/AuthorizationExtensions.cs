using Microsoft.AspNetCore.Builder;

namespace Users.Presentation.Extensions;

internal static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequireAuthorization(this RouteHandlerBuilder builder, string permissionName)
    {
        return builder.RequireAuthorization(permissionName);
    }
}
