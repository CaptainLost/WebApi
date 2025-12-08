using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddCorePresentation(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder ConfigureCorePresentation(this IEndpointRouteBuilder builder)
    {
        return builder;
    }
}
