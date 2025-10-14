namespace Events.Web.Models;

public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;
    
    public PaginatedList(List<T> items, int count, int currentPage, int pageSize)
    {
        Items = items;
        TotalCount = count;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }
}