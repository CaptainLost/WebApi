---
applyTo: '**/*.cs'
---

**⚠️ CRITICAL: This file MUST be read using `read_file` tool before editing or creating ANY `.cs` file. Failure to do so violates the instruction policy.**

You are an expert in C#, .NET, ASP.NET Core, and scalable web application development. You write functional, maintainable, performant, and testable code following Clean Architecture, CQRS, and C# best practices.

# Clean Architecture Guidelines

## Architecture Overview

This project follows **Clean Architecture** principles combined with **Vertical Slice Architecture** and **CQRS** pattern.

## Project Structure

```
src/
├── Domain/          # Core business logic and entities
├── Application/     # Use cases, commands, queries, and abstractions
├── Infrastructure/  # External concerns (Database, External Services)
├── Presentation/    # API endpoints and controllers
└── Host/            # Application entry point and configuration
```

## Clean Architecture Principles

### 1. Dependency Rule
- Dependencies flow inward: `Presentation → Application → Domain`
- Domain has **NO** dependencies on other layers
- Application depends only on Domain
- Infrastructure implements Application abstractions
- Presentation depends on Application

### 2. SOLID Principles

This project adheres to SOLID principles as the foundation of Clean Architecture:

#### Single Responsibility Principle (SRP)
- Each class should have one reason to change
- Handlers focus on a single use case
- Repositories handle only data access for a specific entity
- Services encapsulate a single business concern

#### Open/Closed Principle (OCP)
- Classes are open for extension but closed for modification
- Use abstractions (interfaces) to allow behavior changes without modifying existing code
- Extend functionality through new implementations, not by changing existing ones

#### Liskov Substitution Principle (LSP)
- Derived classes must be substitutable for their base classes
- Implementations of interfaces must honor the contract
- Repository implementations must maintain expected behavior

#### Interface Segregation Principle (ISP)
- Clients should not depend on interfaces they don't use
- Create specific, focused interfaces rather than large, general ones
- Example: `IUserRepository` with only user-specific operations, not a generic `IRepository<T>`

#### Dependency Inversion Principle (DIP)
- **Core of Clean Architecture**
- High-level modules (Application) should not depend on low-level modules (Infrastructure)
- Both should depend on abstractions (interfaces)
- Application defines interfaces, Infrastructure implements them
- Example: `IUserRepository` (Application) ← `UserRepository` (Infrastructure)

### 3. Layer Responsibilities

#### Domain Layer
- Contains **entities**, **value objects**, and **domain errors**
- Pure business logic with no external dependencies
- Defines `Result<T>` and `Error` types for error handling
- Example: `User` entity, `AuthenticationErrors`

#### Application Layer
- Contains **commands**, **queries**, **handlers**, and **abstractions**
- Defines interfaces (repositories, services) implemented by Infrastructure
- Organizes features by vertical slices (e.g., `Authentication/Login`, `Authentication/Register`)
- No direct dependencies on Infrastructure or Presentation
- Example: `LoginCommand`, `RegisterCommand`, `IUserRepository`

#### Infrastructure Layer
- Implements Application abstractions
- Contains **database context**, **repositories**, **external service integrations**
- EF Core migrations and database configurations
- Example: `UserRepository`, `UsersDbContext`, authentication services

#### Presentation Layer
- Contains **API endpoints** using minimal APIs
- Maps HTTP requests to Application commands/queries
- Handles HTTP-specific concerns (routing, validation, responses)
- Returns appropriate HTTP status codes
- Example: `IEndpoint` implementations, `ErrorResults`

#### Host Layer
- Application entry point (`Program.cs`)
- Dependency injection configuration
- Middleware pipeline setup
- Configuration management (`appsettings.json`)

## Vertical Slice Architecture

### Feature Organization
- Group related functionality together by feature (e.g., `Authentication/Login`)
- Each feature contains:
  - Command/Query definition
  - Handler implementation
  - Validation logic
  - DTOs specific to that feature

### Example Structure
```
Application/
└── Authentication/
    ├── Login/
    │   ├── LoginCommand.cs
    │   └── LoginCommandHandler.cs
    ├── Register/
    │   ├── RegisterCommand.cs
    │   └── RegisterCommandHandler.cs
    └── Logout/
        ├── LogoutCommand.cs
        └── LogoutCommandHandler.cs
```

## CQRS Pattern

### Commands
- Represent **write operations** (Create, Update, Delete)
- Should return `Result<T>` or `Result` from Domain
- Located in feature-specific folders under Application
- Naming: `{Action}Command` (e.g., `RegisterCommand`, `LoginCommand`)

### Queries
- Represent **read operations**
- Should return `Result<T>` with the requested data
- Located in feature-specific folders under Application
- Naming: `{Action}Query` (e.g., `GetSessionQuery`, `GetUserQuery`)

### Handlers
- One handler per command/query
- Naming: `{CommandOrQuery}Handler`
- Should contain business logic or orchestrate domain operations
- Use repositories and services through abstractions

### Example Pattern
```csharp
// Command
public sealed record LoginCommand(string Email, string Password) : ICommand<string>;

// Handler
public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, string>
{
    private readonly IUserRepository m_userRepository;
    
    public async Task<Result<string>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

## Error Handling

### Result Pattern
- Use `Result<T>` and `Result` types from Domain
- Never throw exceptions for business logic failures
- Return explicit error results using domain errors

### Error Types
- Define errors in Domain layer: `{Feature}Errors.cs`
- Example: `AuthenticationErrors.InvalidCredentials`
- Map errors to HTTP responses in Presentation layer

### Example
```csharp
// Domain
public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials = new(
        "Authentication.InvalidCredentials",
        "Invalid email or password");
}

// Handler
return Result.Failure<string>(AuthenticationErrors.InvalidCredentials);

// Presentation
return error.Code switch
{
    "Authentication.InvalidCredentials" => Results.Unauthorized(),
    _ => Results.Problem()
};
```

## Dependency Injection

### Registration Pattern
- Each layer has a `DependencyInjection.cs` file
- Use extension methods: `AddApplication()`, `AddInfrastructure()`, `AddPresentation()`
- Register services in the appropriate layer

### Example
```csharp
// Infrastructure/DependencyInjection.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services)
{
    services.AddScoped<IUserRepository, UserRepository>();
    return services;
}
```

## Best Practices

### General
1. Keep Domain layer pure - no external dependencies
2. Use abstractions (interfaces) in Application, implementations in Infrastructure
3. Organize by feature, not by technical concern
4. Use records for DTOs, commands, and queries

### Naming Conventions for Architecture Components
- Commands: `{Action}Command`
- Queries: `{Action}Query`
- Handlers: `{CommandOrQuery}Handler`
- Repositories: `I{Entity}Repository` / `{Entity}Repository`
- Errors: `{Feature}Errors`
- Private fields: Use Hungarian notation `m_` prefix (e.g., `m_userRepository`, `m_decorated`)

### Example
```csharp
// Handler with Hungarian notation for private fields
internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IAuthenticationService authenticationService) : ICommandHandler<LoginCommand>
{
    private readonly IUserRepository m_userRepository = userRepository;
    private readonly IAuthenticationService m_authenticationService = authenticationService;

    public async Task<Result> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await m_userRepository.GetUserByUsernameAsync(command.Username);
        // ...
    }
}
```

### Testing Strategy
- Domain: Unit tests for business logic
- Application: Test handlers with mocked dependencies
- Integration: Test full vertical slices through API endpoints

### Database
- Use EF Core with Code First approach
- Migrations in Infrastructure layer
- Configure entities using `IEntityTypeConfiguration<T>`
- Keep DbContext in Infrastructure, expose through repository abstractions

## Async/Await Guidelines

### General Rules
- All I/O operations (database, HTTP, file system) **must** be async
- Always pass `CancellationToken` to async methods
- Use `async`/`await` keywords properly - don't block with `.Result` or `.Wait()`

### Naming Convention
- Async methods must have `Async` suffix
- Example: `GetUserAsync()`, `SaveChangesAsync()`, `HandleAsync()`
- Handlers use `HandleAsync()` for consistency with async naming convention

### Example
```csharp
// Correct
public async Task<Result<User>> GetUserAsync(Guid id, CancellationToken cancellationToken)
{
    User? user = await m_dbContext.Users
        .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    
    return user == null 
        ? Result.Failure<User>(UserErrors.NotFound) 
        : Result.Success(user);
}

// Handler example
public async Task<Result<string>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
{
    User? user = await m_userRepository.GetByEmailAsync(command.Email, cancellationToken);
    // ...
}
```

### CancellationToken Usage
- Always accept `CancellationToken` in async methods
- Pass it to all downstream async calls
- Parameter name: `cancellationToken` (full name, not `ct`)

## Validation

### Input Validation
- Validate in **Application layer** before business logic
- Use guard clauses for null checks and basic validation
- Consider FluentValidation for complex validation rules
- Return validation errors as `Result.Failure()` with appropriate domain errors

### Business Validation
- Business rules validation in **Domain layer** or **Handlers**
- Example: "User with this email already exists" is business validation
- Example: "Email format is invalid" is input validation

### Example
```csharp
// Input validation in handler
public async Task<Result<Guid>> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
{
    // Input validation
    if (string.IsNullOrWhiteSpace(command.Email))
    {
        return Result.Failure<Guid>(ValidationErrors.EmailRequired);
    }
    
    // Business validation
    bool emailExists = await m_userRepository.ExistsAsync(command.Email, cancellationToken);
    if (emailExists)
    {
        return Result.Failure<Guid>(AuthenticationErrors.EmailAlreadyExists);
    }
    
    // Business logic
    // ...
}
```

## Security Best Practices

### Error Handling
- **Never** expose internal error details in API responses
- Use generic error messages for security-sensitive operations
- Log detailed errors server-side, return generic messages to client
- Example: Don't say "User not found", say "Invalid credentials"

### Password Security
- Always hash passwords using strong algorithms (BCrypt, Argon2)
- Never store plain text passwords
- Never log passwords or sensitive data
- Use separate errors for "user not found" vs "wrong password" internally, but return same message to client

### Authentication vs Authorization
- **Authentication**: Who are you? (Login, tokens)
- **Authorization**: What can you do? (Permissions, roles)
- Implement in Infrastructure layer
- Use abstractions in Application layer

### Example
```csharp
// Bad - reveals user existence
if (user == null)
{
    return Result.Failure<string>(AuthenticationErrors.UserNotFound);
}
if (!passwordValid)
{
    return Result.Failure<string>(AuthenticationErrors.InvalidPassword);
}

// Good - generic message
if (user == null || !passwordValid)
{
    return Result.Failure<string>(AuthenticationErrors.InvalidCredentials);
}
```

### Database
- Use EF Core with Code First approach
- Migrations in Infrastructure layer
- Configure entities using `IEntityTypeConfiguration<T>`
- Keep DbContext in Infrastructure, expose through repository abstractions

## Anti-Patterns to Avoid

1. ❌ Don't reference Infrastructure from Application
2. ❌ Don't put business logic in Presentation layer
3. ❌ Don't create anemic domain models (data without behavior)
4. ❌ Don't use exceptions for control flow
5. ❌ Don't create generic repositories without clear value
6. ❌ Don't mix commands and queries in the same handler
7. ❌ Don't leak domain entities to Presentation layer - use DTOs

## Code Generation Guidelines for Copilot

When generating new features:
1. Start with the command/query in Application layer
2. Create the handler implementing business logic
3. Define necessary abstractions (repositories, services)
4. Implement abstractions in Infrastructure
5. Create endpoint in Presentation that maps to the command/query
6. Add appropriate error handling and validation
7. Follow existing patterns in the codebase

Remember: **Dependencies flow inward, data flows outward!**
