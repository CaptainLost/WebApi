using Core.Facade;
using Core.Facade.Abstractions;
using Core.Facade.Extensions;
using Users.Facade;

namespace Host;

internal static class ModuleRegistry
{
    private static readonly IModule[] _modules =
    [
        new CoreModule(),
        new UsersModule()
    ];

    private static readonly IModule[] _orderedModules = _modules
        .OrderBy(m => m.Order)
        .ToArray();

    internal static void RegisterModules(WebApplicationBuilder builder)
    {
        foreach (IModule module in _orderedModules)
        {
            module.AddModuleConfiguration(builder.Configuration);
            module.RegisterServices(builder);
        }
    }

    internal static void ConfigureModules(WebApplication app)
    {
        foreach (IModule module in _orderedModules)
        {
            module.ConfigureApplication(app);
        }
    }
}
