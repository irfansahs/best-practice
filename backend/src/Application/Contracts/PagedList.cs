namespace Application.Contracts;

public sealed record PageRequest(int Page = 1, int PageSize = 20)
{
    public int Skip => (Page - 1) * PageSize;
}

public sealed record SortSpec(string Field, bool Descending = false);

public sealed record FilterSpec(string Field, string Operator, string Value);

public sealed record PagedList<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNext)
{
    public static PagedList<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalCount) {
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
        return new(items, page, pageSize, totalCount, totalPages, page < totalPages);
    }
}
