namespace Application.Contracts;

public sealed record ApiMeta(string? TraceId = null, string? Culture = null);

public sealed record ApiResponse<T>(bool Success, T Data, ApiMeta Meta)
{
    public static ApiResponse<T> Ok(T data, ApiMeta? meta = null) => new(true, data, meta ?? new ApiMeta());
}

public sealed record ApiError(string Code, string Message, IReadOnlyDictionary<string, string[]>? Errors = null);
