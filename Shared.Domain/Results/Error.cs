namespace Shared.Domain.Results;

public class Error {
    public ErrorType Type { get; }
    public string Code { get; }
    public string? Message { get; }

    protected Error(ErrorType type, string code, string? message) {
        Type = type;
        Message = message;
        Code = code;
    }

    public static ValidationError Validation(IDictionary<string, string[]> errors)
        => new(errors);

    public static ValidationError Validation(string fieldName, string[] errors)
       => Validation(new Dictionary<string, string[]> {
           { fieldName, errors },
       });

    public static ValidationError Validation(string fieldName, string error)
        => Validation(fieldName, [error]);

    public static Error NotFound(string code, string? message)
        => new(ErrorType.NotFound, code, message);

    public static Error Conflict(string code, string? message)
        => new(ErrorType.Conflict, code, message);
}