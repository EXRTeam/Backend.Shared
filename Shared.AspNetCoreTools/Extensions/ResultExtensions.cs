using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Results;

namespace Shared.AspNetCoreTools.Extensions;

public static class ResultExtensions {
    public static IResult ProblemDetails(this Result source)
        => source.Error.ProblemDetails();

    public static IResult ProblemDetails(this Error error) {
        var (title, detail, statusCode, type) = GetProblemData(error);

        var problemDetails = new ProblemDetails {
            Type = type,
            Title = title,
            Detail = detail,
            Status = statusCode,
        };

        if (error.Type == ErrorType.Validation) {
            var validationError = (ValidationError)error;
            problemDetails.Extensions["errors"] = validationError.Errors;
        }

        return Results.Problem(problemDetails);
    }

    private static (string title, string? detail, int statusCode, string type) GetProblemData(Error error) {
        return error.Type switch {
            ErrorType.Validation => (
                "One or more validation errors",
                null,
                StatusCodes.Status400BadRequest,
                "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1"),

            ErrorType.NotFound => (
                error.Code,
                error.Message,
                StatusCodes.Status404NotFound,
                "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5"),

            ErrorType.Conflict => (
                error.Code,
                error.Message,
                StatusCodes.Status409Conflict,
                "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10"),

            _ => (
                "Internal server error",
                null,
                StatusCodes.Status500InternalServerError,
                "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1")
        };
    }
}
