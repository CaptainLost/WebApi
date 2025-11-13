---
applyTo: '**/*.cs'
---

# Clean Architecture with Modular Monolith

When implementing backend services, follow these Clean Architecture principles to ensure maintainability, scalability, and separation of concerns. This rule is tailored for .NET solutions with a **modular monolith** structure where each module follows Clean Architecture principles.

## 1. Solution Structure

The solution **must** be organized into a modular structure with the following hierarchy:

### Root Level
- `src/Host/` - The main application host that orchestrates all modules
- `src/Modules/` - Contains all business modules (e.g., Authentication, Users, Core)
- `tests/` - Contains test projects for each module

### Module Structure
Each module **must** be organized into six projects (one per layer):
  - `[Module].Domain` - Core business logic, entities, value objects, domain events
  - `[Module].Application` - Use cases, commands, queries, interfaces for external services
  - `[Module].Infrastructure` - Implementations for external services, third-party integrations
  - `[Module].Persistence` - Database access, EF Core configurations, repositories
  - `[Module].Presentation` - API endpoints, minimal API, request/response models
  - `[Module].Facade` - Module registration and configuration (exposes `ModuleExtensions.cs`)

### Special Modules
- **Core Module**: Contains shared abstractions, primitives, and messaging infrastructure used by other modules
- Each project must contain a marker/reference file (e.g., `DomainReference.cs`) for test discovery and architecture validation

### Tests
Tests must be in separate projects per module:
  - `tests/[Module].UnitTests` - For Domain and Application layers only
  - `tests/[Module].IntegrationTests` - For Infrastructure, Persistence, Presentation, and architecture validation

## 2. Dependencies Between Layers

### Within a Module
- **Domain**: has no dependencies (except Core.Domain for shared primitives).
- **Application**: depends only on **Domain** (and Core.Application for shared abstractions).
- **Infrastructure**: depends on **Application** and **Domain**.
- **Persistence**: depends on **Application** and **Domain**.
- **Presentation**: depends only on **Application** (not on Infrastructure or Persistence directly).
- **Facade**: depends on all other layers within the module to orchestrate registration.

### Between Modules
- Modules communicate through well-defined interfaces (typically in Application layer).
- **Core Module** provides shared abstractions that can be referenced by other modules.
- Direct module-to-module dependencies should be minimized; prefer event-driven communication or shared contracts.
- The **Host** project references all module Facades to bootstrap the application.

### Enforcement
- These dependencies **must** be enforced by automated architecture tests (e.g., NetArchTest in `ArchitectureTests.cs`).
- Forbidden dependencies (e.g., EntityFrameworkCore in Presentation/Domain) must be checked by tests.
- Module isolation must be validated to prevent tight coupling between modules.

## 3. Folder and File Structure

- Use a **feature-oriented** (domain-driven) folder structure in each layer (e.g., `Order/`, `Customer/`).
- Do **not** use technical root folders (Entities, ValueObjects, Services, etc.).
- Example modular monolith structure:

```
src/
  Host/
    Program.cs
    appsettings.json
    Extensions/
      HostApplicationBuilderExtensions.cs
      WebApplicationExtensions.cs
  Modules/
    Authentication/
      Authentication.Domain/
        DomainReference.cs
        Errors/
        [Feature]/
      Authentication.Application/
        ApplicationReference.cs
        Abstractions/
        [Feature]/
      Authentication.Infrastructure/
        InfrastructureReference.cs
        Services/
      Authentication.Persistence/
        PersistenceReference.cs
        Configurations/
        Repositories/
      Authentication.Presentation/
        PresentationReference.cs
        Endpoints/
      Authentication.Facade/
        ModuleExtensions.cs
    Users/
      Users.Domain/
      Users.Application/
      Users.Infrastructure/
      Users.Persistence/
      Users.Presentation/
      Users.Facade/
    Core/
      Core.Domain/
        Primitives/
        Messaging/
        Enums/
      Core.Application/
        Abstractions/
      [other Core layers...]
tests/
  Authentication.UnitTests/
    ...
  Authentication.IntegrationTests/
    ArchitectureTests.cs
    ...
  Users.UnitTests/
  Users.IntegrationTests/
```

## 4. Coding Style and Conventions

- Use file-scoped namespaces.
- One type per file.
- Follow Microsoft .NET C# coding conventions.
- Organize files by feature/domain.

## 5. Layer Responsibilities

### Domain Layer
**Contains**: Core business logic and domain model
- **Entities**: Objects with unique identity (e.g., User, Order)
- **Value Objects**: Immutable objects defined by their attributes (e.g., Money, Address)
- **Domain Events**: Events that represent something significant that happened in the domain
- **Domain Services**: Business logic that doesn't naturally fit within entities or value objects
- **Interfaces**: Abstractions for repositories and domain-specific services
- **Exceptions**: Domain-specific exceptions for business rule violations
- **Enums**: Domain-specific enumerations
- **No dependencies** on other layers (except Core.Domain for shared primitives)
- **No infrastructure concerns** (database, external APIs, etc.)

### Application Layer
**Orchestrates the domain** and defines use cases
- **Application Services**: Orchestration of domain logic and use cases (commands/queries handlers)
- **CQRS**: Commands (write operations) and Queries (read operations)
- **DTOs**: Data transfer objects for application boundaries
- **Interfaces**: Abstractions for application-specific services (e.g., IEmailService, INotificationService)
- **Depends only on Domain** (and Core.Application for shared abstractions)
- **No business logic** - delegates to domain services and entities
- **No infrastructure concerns** - uses interfaces for external dependencies

### Infrastructure Layer
**Implements interfaces for external systems**
- **External Systems Integration**: Third-party APIs, messaging systems
- **Email Providers**: SMTP, SendGrid, etc.
- **Storage Services**: Blob storage, file systems
- **Identity**: Authentication and authorization implementations
- **System Clock**: Time providers, date/time services
- **Depends on Application and Domain**
- **No database access** - database concerns belong in Persistence layer

### Persistence Layer
**Handles all database-related concerns** (separated from Infrastructure)
- **Database Context**: EF Core DbContext
- **Entity Configurations**: Fluent API configurations for EF Core
- **Repositories**: Implementation of repository interfaces from Domain/Application
- **Migrations**: Database migrations
- **Database-specific implementations**: Queries, stored procedures
- **Depends on Application and Domain**
- **No business logic** - only data access and persistence

### Presentation Layer
**API endpoints and request/response handling**
- **Minimal API Endpoints**: HTTP endpoints using ASP.NET Core Minimal APIs
- **Request/Response Models**: DTOs for API contracts
- **Input Validation**: Basic request validation
- **Depends only on Application** (not on Infrastructure or Persistence directly)
- **No business logic** - delegates to application services

### Facade Layer
**Module registration and configuration**
- **Module Registration**: `ModuleExtensions.cs` to configure services and pipeline
- **Dependency Injection Setup**: Registers all module services
- **Depends on all other layers** within the module to orchestrate registration

### General Guidelines
- Use dependency injection for all cross-layer dependencies
- Avoid circular dependencies
- Do not use a mediator library; call service methods directly from the Presentation layer
- Follow the dependency rule: outer layers depend on inner layers, never the reverse

## 6. Testing and Architecture Validation

- **Unit Tests**: In `tests/[Module].UnitTests/`, for Domain and Application layers only. Use xUnit v3 and FakeItEasy for mocks.
- **Integration Tests**: In `tests/[Module].IntegrationTests/`, for Infrastructure, Persistence, and Presentation layers. Use Testcontainers/Microcks for advanced scenarios.
- **Architecture Tests**: Must be present in `ArchitectureTests.cs` and:
  - Enforce allowed/forbidden dependencies between layers
  - Check for forbidden dependencies (e.g., EF Core in Presentation/Domain)
  - Validate module isolation
  - Optionally, check for immutability in Domain
- Always write tests before implementation (TDD).

## 7. Architecture Testing Example

To enforce and validate architecture rules, add automated tests in `tests/[project].IntegrationTests/ArchitectureTests.cs` using [NetArchTest](https://github.com/BenMorris/NetArchTest). Example:

```csharp
using System.Reflection;
using NetArchTest.Rules;
using Xunit;
using Xunit.Abstracts;

using Order.Application;
using Order.Domain;
using Order.Infrastructure;

namespace Order.IntegrationTests;

public class ArchitectureTests
{
    private static string EntityFrameworkCore = "Microsoft.EntityFrameworkCore";
    private const string ApiNamespace = "Api";
    private const string ApplicationNamespace = "Application";
    private const string DomainNamespace = "Domain";
    private const string InfrastructureNamespace = "Infrastructure";

    private static readonly Assembly ApiAssembly = typeof(Program).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ApplicationReference).Assembly;
    private static readonly Assembly DomainAssembly = typeof(DomainReference).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(InfrastructureReference).Assembly;

    public ITestOutputHelper TestOutputHelper { get; }

    public ArchitectureTests(ITestOutputHelper testOutputHelper)
    {
        this.TestOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Api_ShouldOnlyDependOn_Application()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespace(ApiNamespace)
            .Should().HaveDependencyOn(ApplicationNamespace)
            .And()
            .NotHaveDependencyOn(DomainNamespace)
            .And()
            .NotHaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, $"{ApiNamespace} should only depend on {ApplicationNamespace}");
    }

    // ...other architecture tests for Application, Infrastructure, Domain, and forbidden dependencies...
}
```

- Adapt namespaces, assemblies, and rules to your solution.
- Add tests to check for forbidden dependencies (e.g., EntityFrameworkCore in Presentation/Domain) and for immutability in Domain types if relevant.
- Run these tests with `dotnet test` to ensure architecture rules are enforced after every change.

## Additional Guidelines

1. Use dependency injection to manage dependencies across layers.
2. Avoid circular dependencies between layers.
3. Write unit tests for **Domain** and **Application** layers.
4. Use integration tests for **Infrastructure**, **Persistence**, and **Presentation** layers.
5. Follow SOLID principles within each layer.
6. Avoid using a mediator library; instead, directly call service methods from the **Presentation** layer.
7. Each module should be self-contained and expose a clear contract through its Facade.
8. Minimize direct dependencies between modules; prefer event-driven communication.
9. The **Host** project should only reference module Facades, not internal layers.

# References
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/TheCleanArchitecture.html)
