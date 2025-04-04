using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Recruit.Api.Domain.Models;

public class PaginatedList<T>(List<T> items, int count, int pageIndex, int pageSize)
{
    public List<T> Items { get; private set; } = items;
    public int TotalCount { get; private set; } = count;
    public int PageIndex { get; private set; } = pageIndex;
    public int PageSize { get; private set; } = pageSize;
    public int TotalPages
    {
        get 
        {
            return (int)Math.Ceiling(TotalCount / (double)PageSize);
        }
    }
    public bool HasPreviousPage
    {
        get
        {
            return PageIndex > 1;
        }
    }
    public bool HasNextPage
    {
        get
        {
            return PageIndex < TotalPages;
        }
    }
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int total, int pageIndex, int pageSize)
    {
        var items = await source.ToListAsync();
        return new PaginatedList<T>(items, total, pageIndex, pageSize);
    }
}