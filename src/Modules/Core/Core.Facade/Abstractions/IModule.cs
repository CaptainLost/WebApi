using Microsoft.AspNetCore.Builder;

namespace Core.Facade.Abstractions;

public interface IModule
{
    string Name { get; }

    int Order { get; }

    void RegisterServices(WebApplicationBuilder builder);
    void ConfigureApplication(WebApplication app);
}
