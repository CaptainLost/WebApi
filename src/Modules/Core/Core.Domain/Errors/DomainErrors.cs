using Core.Domain.Messaging;

namespace Core.Domain.Errors;

public static class DomainErrors
{
    public static Error TooLong(string objectName) => new(
        Code: "Domain.TooLong",
        Description: $"{objectName} is too long");

    public static Error IsEmpty(string objectName) => new(
        Code: "Domain.IsEmpty",
        Description: $"{objectName} is empty");

    public static Error InvalidFormat(string objectName) => new(
        Code: "Domain.InvalidFormat",
        Description: $"{objectName} format is invalid");
}