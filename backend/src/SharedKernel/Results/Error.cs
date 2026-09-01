namespace SharedKernel.Results;

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type,
    IReadOnlyDictionary<string, string[]>? ValidationErrors = null)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Unexpected);

    public bool IsNone => this == None;

    public static Error Validation(
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? validationErrors = null) =>
        new(code, message, ErrorType.Validation, validationErrors);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);

    public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);

    public static Error Unexpected(string code, string message) => new(code, message, ErrorType.Unexpected);
}
