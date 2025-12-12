using System.Reflection;
using Common.ArchitectureTests;

namespace Users.ArchitectureTests;

public sealed class LayerDependencyTests : BaseArchitectureTests
{
    private static readonly Assembly DomainAssembly = Users.Domain.AssemblyReference.Assembly;
    private static readonly Assembly ApplicationAssembly = Users.Application.AssemblyReference.Assembly;
    private static readonly Assembly InfrastructureAssembly = Users.Infrastructure.AssemblyReference.Assembly;
    private static readonly Assembly PersistenceAssembly = Users.Persistence.AssemblyReference.Assembly;
    private static readonly Assembly PresentationAssembly = Users.Presentation.AssemblyReference.Assembly;
    private static readonly Assembly FacadeAssembly = Users.Facade.AssemblyReference.Assembly;

    private const string EntityFrameworkCoreName = "Microsoft.EntityFrameworkCore";

    #region Domain Layer

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_AnyOtherLayers()
    {
        AssertLayerDoesNotHaveDependencyOnAny(DomainAssembly,
            ApplicationAssembly,
            InfrastructureAssembly,
            PersistenceAssembly,
            PresentationAssembly,
            FacadeAssembly);
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_EntityFrameworkCore()
    {
        AssertLayerDoesNotHaveDependencyOn(DomainAssembly,
            EntityFrameworkCoreName);
    }

    #endregion

    #region Application Layer

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_AnyOtherLayers()
    {
        AssertLayerDoesNotHaveDependencyOnAny(ApplicationAssembly,
            InfrastructureAssembly,
            PersistenceAssembly,
            PresentationAssembly,
            FacadeAssembly);
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_EntityFrameworkCore()
    {
        AssertLayerDoesNotHaveDependencyOn(ApplicationAssembly,
            EntityFrameworkCoreName);
    }

    #endregion

    #region Infrastructure Layer

    [Fact]
    public void InfrastructureLayer_ShouldNotHaveDependencyOn_AnyOtherLayers()
    {
        AssertLayerDoesNotHaveDependencyOnAny(InfrastructureAssembly,
            PersistenceAssembly,
            PresentationAssembly,
            FacadeAssembly);
    }

    #endregion

    #region Persistence Layer

    [Fact]
    public void PersistenceLayer_ShouldNotHaveDependencyOn_AnyOtherLayers()
    {
        AssertLayerDoesNotHaveDependencyOnAny(PersistenceAssembly,
            InfrastructureAssembly,
            PresentationAssembly,
            FacadeAssembly);
    }

    #endregion

    #region Presentation Layer

    [Fact]
    public void PresentationLayer_ShouldNotHaveDependencyOn_InfrastructureOrPersistence()
    {
        AssertLayerDoesNotHaveDependencyOnAny(PresentationAssembly,
            InfrastructureAssembly,
            PersistenceAssembly,
            FacadeAssembly);
    }

    [Fact]
    public void PresentationLayer_ShouldNotHaveDependencyOn_EntityFrameworkCore()
    {
        AssertLayerDoesNotHaveDependencyOn(PresentationAssembly,
            EntityFrameworkCoreName);
    }

    #endregion
}