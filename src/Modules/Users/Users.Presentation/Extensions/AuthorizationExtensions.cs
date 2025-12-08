using Microsoft.AspNetCore.Builder;
using Users.Domain.Users;

namespace Users.Presentation.Extensions;

internal static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequireAuthorization(this RouteHandlerBuilder builder, Permission permission)
    {
        return builder.RequireAuthorization(permission.Name);
    }
}
