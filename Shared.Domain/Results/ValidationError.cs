namespace Shared.Domain.Results;

public class ValidationError : Error {
    public IDictionary<string, string[]> Errors { get; }

    internal ValidationError(IDictionary<string, string[]> errorsCollection)
        : base(ErrorType.Validation, code: string.Empty, message: null) {
        Errors = errorsCollection;
    }
}
