using System.Reflection;
using Common.ArchitectureTests;
using NetArchTest.Rules;

namespace Host.ArchitectureTests;

public abstract class CommonPersistenceTests
{
    protected static void AssertSpecificationsShouldBeSealed(Assembly assembly)
    {
        Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Specification<>))
            .Should()
            .BeSealed()
            .GetResult()
            .ShouldBeSuccessful();
    }

    protected static void AssertSpecificationsShouldNotBePublic(Assembly assembly)
    {
        Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Specification<>))
            .Should()
            .NotBePublic()
            .GetResult()
            .ShouldBeSuccessful();
    }

    protected static void AssertSpecificationsShouldHaveNameEndingWithSpecification(Assembly assembly)
    {
        Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Specification<>))
            .Should()
            .HaveNameEndingWith("Specification")
            .GetResult()
            .ShouldBeSuccessful();
    }
}
