namespace Trivista.LoanApp.ApplicationCore.Commons.Pagination;

public sealed class PaginationInfo<T>
{
    public List<T> Data { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    public PaginationInfo(List<T> data, int currentPage, int pageSize, int totalItems, int totalPages)
    {
        Data = data;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = totalPages;
    }
}