using System.Reflection;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace Core.Presentation.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapEndpointsFromAssembly(this IEndpointRouteBuilder builder,
        Assembly assembly, RouteGroupBuilder group)
    {
        IEnumerable<Type> endpointTypes = assembly
            .GetTypes()
            .Where(type => type.IsClass &&
                          !type.IsAbstract &&
                          typeof(IEndpoint).IsAssignableFrom(type));

        foreach (Type endpointType in endpointTypes)
        {
            IEndpoint? endpoint = builder.ServiceProvider.GetService(endpointType) as IEndpoint;

            endpoint?.MapEndpoint(group);
        }

        return builder;
    }
}
