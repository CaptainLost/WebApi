using Core.Domain.Messaging;
using Microsoft.AspNetCore.Http;

namespace Core.Presentation.Common;

public static class ErrorResults
{
    public static IResult FromError(Error error) => Results.Json(
        data: new ErrorResponse(error.Code, error.Description),
        statusCode: (int)error.StatusCode);
}