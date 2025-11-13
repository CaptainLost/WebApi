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
- Use meaningful, descriptive names; avoid abbreviations.

## Formatting
- Use 4 spaces for indentation (no tabs).
- Use file-scoped namespaces to simplify structure and improve readability.
- Add a blank line between method definitions.
- Place opening braces on a new line for methods, properties, and types (unless using file-scoped namespaces, then follow the file-scoped style).

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
- **Prefer explicit types** for local variable declarations to improve code clarity and readability.
- Avoid using `var` unless the type is immediately obvious from the right-hand side (e.g., `new ClassName()`).
- Using explicit types makes the code more maintainable and easier to understand at a glance.

### Example
```csharp
// Incorrect - avoid var for primitives and non-obvious types
var x = 1;
var y = 2.0;
var z = "Hello";
var item = GetProductBacklogItem();

// Correct - use explicit types
int x = 1;
double y = 2.0;
string z = "Hello";
ProductBacklogItem item = GetProductBacklogItem();

// Acceptable - type is obvious from the constructor
var item = new ProductBacklogItem("Test", "Test", 1, 1, 1);
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

## Comments & Documentation
- Use XML documentation comments (`///`) for public APIs.
- Write comments to explain why, not what, when necessary.
- Remove commented-out code before committing.

## Null Checks & Exceptions
- Use guard clauses for argument validation.
- Use `nameof` for parameter names in exceptions.
- **Prefer `== null` and `!= null` over `is null` and `is not null`** for null checks to maintain consistency across the codebase.

### Example
```csharp
// Incorrect - avoid is null / is not null
if (user is null)
{
    return UserErrors.NotFound();
}

if (order is not null)
{
    // process order
}

// Correct - use == null / != null
if (user == null)
{
    return UserErrors.NotFound();
}

if (order != null)
{
    // process order
}
```

## Modern C# Features
- Use pattern matching and expression-bodied members where appropriate.
- Prefer object and collection initializers.

# References
- Adhere to Microsoft's [coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).