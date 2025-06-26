using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

internal static class PageInfoExtensions
{
    public static PageInfo ToPageInfo(this PaginatedList<ApplicationReviewEntity> paginatedList) 
        => new(paginatedList.TotalCount, paginatedList.PageIndex, paginatedList.PageSize, paginatedList.TotalPages, paginatedList.HasPreviousPage, paginatedList.HasNextPage);

    public static PageInfo ToPageInfo(this PaginatedList<VacancyDetail> paginatedList)
        => new(paginatedList.TotalCount, paginatedList.PageIndex, paginatedList.PageSize, paginatedList.TotalPages, paginatedList.HasPreviousPage, paginatedList.HasNextPage);
}