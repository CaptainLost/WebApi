using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace Common.ArchitectureTests;

public abstract class BaseArchitectureTests
{
    protected static void AssertLayerDoesNotHaveDependencyOnAny(
        Assembly layerAssembly,
        params Assembly[] forbiddenAssemblies)
    {
        string?[] assemblyNames = forbiddenAssemblies.Select(x => x.GetName().Name)
            .ToArray();

        Types.InAssembly(layerAssembly)
            .Should()
            .NotHaveDependencyOnAny(assemblyNames)
            .GetResult()
            .FailingTypesShouldBeEmpty();
    }

    protected static void AssertLayerDoesNotHaveDependencyOnAny(
        Assembly layerAssembly,
        params string[] forbiddenAssemblieNames)
    {
        Types.InAssembly(layerAssembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenAssemblieNames)
            .GetResult()
            .FailingTypesShouldBeEmpty();
    }

    protected static void AssertLayerDoesNotHaveDependencyOn(
        Assembly layerAssembly,
        Assembly forbiddenAssembly)
    {
        Types.InAssembly(layerAssembly)
            .Should()
            .NotHaveDependencyOn(forbiddenAssembly.GetName().Name)
            .GetResult()
            .FailingTypesShouldBeEmpty();
    }

    protected static void AssertLayerDoesNotHaveDependencyOn(
        Assembly layerAssembly,
        string forbiddenAssemblyName)
    {
        Types.InAssembly(layerAssembly)
            .Should()
            .NotHaveDependencyOn(forbiddenAssemblyName)
            .GetResult()
            .FailingTypesShouldBeEmpty();
    }
}