namespace SharedKernel.Results;

public sealed record ProblemDetails(
    int Status,
    string Title,
    string Detail,
    string Type,
    IReadOnlyDictionary<string, string[]>? Errors = null);

public static class ResultExtensions
{
    public static TOut Match<TOut>(this Result result, Func<TOut> onSuccess, Func<Error, TOut> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);

    public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<Error, TOut> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);

    public static Result Map(this Result result, Action onSuccess) =>
        result.IsSuccess ? Result.Success() : Result.Failure(result.Error);

    public static Result<TOut> Map<TOut>(this Result result, Func<TOut> mapper) =>
        result.IsSuccess ? Result<TOut>.Success(mapper()) : Result<TOut>.Failure(result.Error);

    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper) =>
        result.IsSuccess ? Result<TOut>.Success(mapper(result.Value)) : Result<TOut>.Failure(result.Error);

    public static ProblemDetails ToProblemDetails(this Result result) =>
        result.IsSuccess
            ? throw new InvalidOperationException("Cannot convert a successful result to problem details.")
            : result.Error.ToProblemDetails();

    public static ProblemDetails ToProblemDetails(this Error error) => new(
        error.Type.ToStatusCode(),
        error.Type.ToTitle(),
        error.Message,
        $"https://httpstatuses.com/{error.Type.ToStatusCode()}");

    private static int ToStatusCode(this ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.BadRequest,
        ErrorType.NotFound => StatusCodes.NotFound,
        ErrorType.Conflict => StatusCodes.Conflict,
        ErrorType.Forbidden => StatusCodes.Forbidden,
        ErrorType.Unauthorized => StatusCodes.Unauthorized,
        _ => StatusCodes.InternalServerError
    };

    private static string ToTitle(this ErrorType type) => type switch
    {
        ErrorType.Validation => "Validation Error",
        ErrorType.NotFound => "Not Found",
        ErrorType.Conflict => "Conflict",
        ErrorType.Forbidden => "Forbidden",
        ErrorType.Unauthorized => "Unauthorized",
        _ => "Unexpected Error"
    };

    private static class StatusCodes
    {
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int Conflict = 409;
        public const int InternalServerError = 500;
    }
}
