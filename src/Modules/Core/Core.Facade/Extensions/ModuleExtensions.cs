using Core.Facade.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Core.Facade.Extensions;

public static class ModuleExtensions
{
    public static IConfigurationBuilder AddModuleConfiguration(
        this IModule module,
        IConfigurationBuilder configurationBuilder)
    {
        string assemblyLocation = module.GetType().Assembly.Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation) ?? throw new InvalidOperationException($"Cannot determine directory for assembly: {assemblyLocation}");

        string moduleName = module.Name.ToLowerInvariant();
        string configFile = Path.Combine(assemblyDirectory, $"{moduleName}.configuration.json");
        string devConfigFile = Path.Combine(assemblyDirectory, $"{moduleName}.configuration.Development.json");

        if (File.Exists(configFile))
        {
            configurationBuilder.AddJsonFile(configFile, optional: false, reloadOnChange: true);
        }

        if (File.Exists(devConfigFile))
        {
            configurationBuilder.AddJsonFile(devConfigFile, optional: true, reloadOnChange: true);
        }

        return configurationBuilder;
    }
}
