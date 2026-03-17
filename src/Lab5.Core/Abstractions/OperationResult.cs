namespace Lab5.Core.Abstractions;

public abstract record OperationResult<T>
{
    private OperationResult() { }

    public sealed record Success(T Result) : OperationResult<T>;

    public sealed record BadRequest(string Message) : OperationResult<T>;

    public sealed record Unauthorized(string Message) : OperationResult<T>;

    public sealed record InternalError(string Message) : OperationResult<T>;
}