using System.Reflection;
using Xunit;

namespace Host.ArchitectureTests;

public sealed class NamingConventionTests : CommonPersistenceTests
{
    private static readonly Assembly HostAssembly = typeof(Host.ModuleRegistry).Assembly;

    private static readonly Assembly[] AllModuleAssemblies = HostAssembly
        .GetReferencedAssemblies()
        .Select(Assembly.Load)
        .Where(assembly =>
        {
            string assemblyName = assembly.GetName().Name ?? string.Empty;
            return assemblyName.EndsWith(".Facade");
        })
        .ToArray();

    [Fact]
    public void Specifications_ShouldHaveNameEndingWithSpecification()
    {
        foreach (Assembly assembly in AllModuleAssemblies)
        {
            AssertSpecificationsShouldHaveNameEndingWithSpecification(assembly);
        }
    }

    [Fact]
    public void Specifications_ShouldBeSealed()
    {
        foreach (Assembly assembly in AllModuleAssemblies)
        {
            AssertSpecificationsShouldBeSealed(assembly);
        }
    }

    [Fact]
    public void Specifications_ShouldNotBePublic()
    {
        foreach (Assembly assembly in AllModuleAssemblies)
        {
            AssertSpecificationsShouldNotBePublic(assembly);
        }
    }
}
