using FluentValidation.Results;
using Shared.Domain.Results;

namespace Shared.Application;

internal static class ValidationResultExtensions { 
    public static TResult ToResult<TResult>(this ValidationResult validationResult) where TResult : Result {
        var error = Error.Validation(validationResult.ToDictionary());

        if (typeof(TResult) == typeof(Result)) {
            return (TResult)Result.Failure(error);
        }

        var result = typeof(Result<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
                .GetMethod(nameof(Result.Failure))!
                .Invoke(null, [error])!;

        return (TResult)result;
    }
}
