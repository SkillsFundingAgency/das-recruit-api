using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

internal static class PageInfoExtensions
{
    public static PageInfo ToPageInfo<T>(this PaginatedList<T> paginatedList)
    {
        return new PageInfo(paginatedList.TotalCount, paginatedList.PageIndex, paginatedList.PageSize, paginatedList.TotalPages, paginatedList.HasPreviousPage, paginatedList.HasNextPage);
    }
}