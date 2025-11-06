---
applyTo: '**/*.cs'
---

# Coding Style

## General Guidelines
- Follow the official Microsoft .NET C# coding conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
- Prefer clarity and readability over brevity.
- Use consistent formatting and naming throughout the codebase.

## Naming Conventions
- Use `PascalCase` for class, method, and property names.
- Use `camelCase` for local variables and method parameters.
- Use `ALL_CAPS` for constants.
- Prefix interfaces with `I` (e.g., `IOrderService`).
- **Use Hungarian notation `m_` prefix for private fields** (e.g., `m_userRepository`, `m_logger`).
- Use meaningful, descriptive names; avoid abbreviations.

### Example: Private Fields with Hungarian Notation
```csharp
// Good
private readonly IUserRepository m_userRepository;
private readonly ILogger<MyClass> m_logger;
private string m_cachedValue;

// Bad - no prefix
private readonly IUserRepository _userRepository;
private readonly IUserRepository userRepository;
```

## Formatting
- Use 4 spaces for indentation (no tabs).
- Use file-scoped namespaces to simplify structure and improve readability.
- Add a blank line between method definitions.
- Place opening braces on a new line for methods, properties, and types (unless using file-scoped namespaces, then follow the file-scoped style).
- **Always use braces** `{}` for control statements (`if`, `for`, `while`, `foreach`), even for single-line statements.

### Example: Always Use Braces
```csharp
// Good
if (user == null)
{
    return Result.Failure(UserErrors.NotFound);
}

if (condition)
{
    DoSomething();
}

// Bad - no braces
if (user == null)
    return Result.Failure(UserErrors.NotFound);

if (condition)
    DoSomething();
```

### Example: File-Scoped Namespaces
```csharp
// Before
namespace MyNamespace
{
    public class ExampleClass
    {
        // ...existing code...
    }
}
// After
namespace MyNamespace;

public class ExampleClass
{
    // ...existing code...
}
```
- All new files must use file-scoped namespaces. Refactor existing files during updates or maintenance.

## Variable Declaration
- Always use explicit types for variable declarations.
- Avoid using `var` unless the type is immediately obvious from the right side of the assignment.

### Example
```csharp
// Preferred
int x = 1;
double y = 2.0;
string z = "Hello";
ProductBacklogItem item = new ProductBacklogItem("Test", "Test", 1, 1, 1);

// Acceptable (type is obvious)
var item = new ProductBacklogItem("Test", "Test", 1, 1, 1);

// Avoid
var x = 1;
var y = 2.0;
var z = "Hello";
```

## Sealed Classes
- Make classes `sealed` by default. If a class needs to be inherited, mark it as `virtual` explicitly.

## Use Nameof with Exceptions
- When throwing exceptions, use `nameof` to refer to the parameter name instead of hardcoding it.

### Example
```csharp
// Before
throw new ArgumentNullException("parameterName");
// After
throw new ArgumentNullException(nameof(parameterName));
```

## Code Structure
- One type per file (class, interface, enum, etc.).
- Organize files by feature/domain when possible.
- Group using directives at the top of the file, outside the namespace.
- Place related types in the same namespace.
- Use partial classes only when necessary (e.g., for code generation).

## Access Modifiers

### Use `public` for:
- **Interfaces** - contracts that define abstractions (e.g., `IUserRepository`, `ICommandHandler`)
- **Commands and Queries** - part of Application's public API
- **Domain types** - core business objects (e.g., `Result`, `Error`, `User`)
- **Static utility classes** - extension methods, DI registration (e.g., `DependencyInjection`)
- **Error definitions** - domain/application errors (e.g., `AuthenticationErrors`)

### Use `internal` for:
- **Implementations** - concrete classes that implement interfaces (e.g., `UserRepository`, `AuthenticationService`)
- **Handlers** - command and query handlers (e.g., `LoginCommandHandler`)
- **Endpoints** - API endpoint implementations (e.g., `LoginEndpoint`)
- **Decorators** - cross-cutting concern decorators (e.g., `CommandHandlerLoggingDecorator`)
- **Infrastructure details** - DbContext, configurations, internal services

### Rationale
- `public` exposes the **contract** (what can be used)
- `internal` hides the **implementation** (how it's done)
- This enforces Clean Architecture - depend on abstractions, not implementations

### Example
```csharp
// Public - contract (Application layer)
public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
}

// Internal - implementation (Infrastructure layer)
internal sealed class UserRepository : IUserRepository
{
    private readonly UserManager<User> m_userManager;
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await m_userManager.FindByNameAsync(username);
    }
}
```

## Comments & Documentation
- Use XML documentation comments (`///`) for public APIs.
- Write comments to explain why, not what, when necessary.
- Remove commented-out code before committing.

## Null Checks & Exceptions
- Use guard clauses for argument validation.
- Use `nameof` for parameter names in exceptions.

## Modern C# Features
- Use pattern matching and expression-bodied members where appropriate.
- Prefer object and collection initializers.

## Async/Await

### General Rules
- Use `async`/`await` for all I/O operations
- Always include `Async` suffix in method names (including handler methods: `HandleAsync`)
- Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` - they can cause deadlocks
- Avoid `async void` - use `async Task` instead (except for event handlers)

### ConfigureAwait
- In library code, use `ConfigureAwait(false)` to avoid capturing context
- In ASP.NET Core, `ConfigureAwait(true)` is default and usually appropriate
- This project: Don't use `ConfigureAwait` unless there's a specific performance reason

### Example
```csharp
// Good
public async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken)
{
    return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
}

// Bad - blocking
public User? GetUser(Guid id)
{
    return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id).Result; // Deadlock risk!
}

// Bad - async void
public async void ProcessDataAsync() // Should be async Task
{
    await _service.ProcessAsync();
}
```

## Null Safety

### Nullable Reference Types
- Nullable reference types are **enabled** in this project
- Use `?` for nullable reference types: `string?`, `User?`
- Use `!` null-forgiving operator sparingly and only when you're certain

### Null Handling Patterns
```csharp
// Null-conditional operator
string? name = user?.Name;

// Null-coalescing operator
string displayName = user?.Name ?? "Unknown";

// Null-coalescing assignment
name ??= "Default";

// Null checks - prefer == null and always use braces
if (user == null)
{
    return Result.Failure(UserErrors.NotFound);
}

if (user != null)
{
    ProcessUser(user);
}
```

### Avoid
```csharp
// Avoid - use == null instead
if (user is null)
{
    return;
}

if (user is not null)
{
    ProcessUser(user);
}

// Bad - no braces
if (user == null)
    return;

// Bad - risky use of null-forgiving operator
string name = user!.Name; // Only if you're 100% certain user is not null
```

## Expression-Bodied Members

Use expression-bodied members for simple one-line implementations.

### When to Use
```csharp
// Properties
public string FullName => $"{FirstName} {LastName}";

// Methods (simple)
public bool IsActive() => Status == UserStatus.Active;

// Constructors (when simple)
public User(string name) => Name = name;

// Read-only properties
public int Count => _items.Count;
```

### When NOT to Use
```csharp
// Multiple statements - use block body
public void ProcessUser(User user)
{
    ValidateUser(user);
    SaveUser(user);
    NotifyUser(user);
}

// Complex logic - use block body for clarity
public decimal CalculateTotal()
{
    decimal subtotal = Items.Sum(i => i.Price);
    decimal tax = subtotal * TaxRate;
    return subtotal + tax;
}
```

## String Handling

### Prefer String Interpolation
```csharp
// Good - string interpolation
string message = $"User {user.Name} has {user.Points} points";

// Good - for expressions
string status = $"Status: {user.IsActive ? "Active" : "Inactive"}";

// Avoid - concatenation
string message = "User " + user.Name + " has " + user.Points + " points";
```

### Use nameof for Identifiers
```csharp
// Good
throw new ArgumentNullException(nameof(user));
Console.WriteLine($"{nameof(User)}.{nameof(User.Email)} is required");

// Bad
throw new ArgumentNullException("user");
Console.WriteLine("User.Email is required");
```

### Verbatim Strings for Paths and Multi-line
```csharp
// File paths
string path = @"C:\Users\Documents\file.txt";

// Multi-line strings
string sql = @"
    SELECT * 
    FROM Users 
    WHERE IsActive = 1";

// Raw string literals (C# 11+) for JSON/HTML
string json = """
    {
        "name": "John",
        "age": 30
    }
    """;
```

# References
- Adhere to Microsoft's [coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).