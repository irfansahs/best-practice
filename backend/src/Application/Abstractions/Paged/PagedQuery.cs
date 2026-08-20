namespace Application.Abstractions.Paged;

public abstract record PagedQuery(int Page = 1, int PageSize = 20)
{
    public int SafePage => Page < 1 ? 1 : Page;
    public int SafePageSize => PageSize is < 1 or > 100 ? 20 : PageSize;
    public int Skip => (SafePage - 1) * SafePageSize;
}
