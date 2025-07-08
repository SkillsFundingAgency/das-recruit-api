using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class PaginatedListExtensions
{
    public static PagedResponse<TResult> ToPagedResponse<T, TResult>(this PaginatedList<T> paginatedList, Func<T, TResult> mapper)
    {
        var items = paginatedList.Items.Select(mapper);
        return new PagedResponse<TResult>(paginatedList.ToPageInfo(), items);
    }
}