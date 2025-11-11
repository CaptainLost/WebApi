using Core.Domain.Messaging;
using Microsoft.AspNetCore.Http;

namespace Core.Presentation.Common;

public static class ErrorResults
{
    public static IResult FromError(Error error, int statusCode) => Results.Json(
        data: new ErrorResponse(error.Code, error.Description),
        statusCode: statusCode);
}