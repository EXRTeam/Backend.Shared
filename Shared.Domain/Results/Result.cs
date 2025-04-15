namespace Shared.Domain.Results;

public class Result {
    private readonly Error? error;

    public Error Error => error ?? throw new NullReferenceException("Result was success, no errors");

    public bool IsSuccess => error == null;
    public bool IsFailure => error != null;

    protected Result() {
        error = null;
    } 

    protected Result(Error error) {
        this.error = error;
    }

    public static readonly Result Success = new();
    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(Error error) => new(error);
}

public sealed class Result<TValue> : Result {
    private readonly TValue? value;

    public TValue Value => IsSuccess
        ? value!
        : throw new NullReferenceException("You try access to null object value");

    private Result(TValue value) {
        this.value = value;
    }

    private Result(Error error) : base(error) { }

    public new static Result<TValue> Success(TValue value) => new(value);
    public new static Result<TValue> Failure(Error error) => new(error);

    public static implicit operator TValue(Result<TValue> result) => result.Value;
    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(error);
}