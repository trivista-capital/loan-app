using Microsoft.EntityFrameworkCore;

namespace Trivista.LoanApp.ApplicationCore.Commons.Pagination;

public static class PaginationData
{
    public static async Task<PaginationInfo<T>> PaginateAsync<T>(
        IQueryable<T> query,
        int page,
        int pageSize)
    {
        if (pageSize < 1)
            pageSize = 10;

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (page > totalPages)
            page = totalPages;

        if (page < 1)
            page = 1;
        
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginationInfo<T>(items, page, pageSize, totalItems, totalPages);
    }
}