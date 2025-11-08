using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Endpoints.Authentication;

internal static class AuthenticationEndpointExtensions
{
    internal static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder
            .MapGroup(ApiRoutes.Auth.Base)
            .WithTags(EndpointTag.Authentication);

        IServiceProvider services = builder.ServiceProvider;

        services.GetRequiredService<LoginEndpoint>().MapEndpoint(group);
        services.GetRequiredService<LogoutEndpoint>().MapEndpoint(group);
        services.GetRequiredService<RegisterEndpoint>().MapEndpoint(group);
        services.GetRequiredService<GetSessionEndpoint>().MapEndpoint(group);

        return builder;
    }
}
