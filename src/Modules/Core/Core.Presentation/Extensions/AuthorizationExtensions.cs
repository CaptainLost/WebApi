using Core.Domain.Primitives;
using Microsoft.AspNetCore.Builder;

namespace Core.Presentation.Extensions;

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequireAuthorization<TEnum>(
        this RouteHandlerBuilder builder,
        Enumeration<TEnum> permission)
        where TEnum : Enumeration<TEnum>
    {
        return builder.RequireAuthorization(permission.Name);
    }
}
