﻿using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Extensions;

public static class PageInfoExtensions
{
    public static PageInfo ToPageInfo(this PaginatedList<ApplicationReviewEntity> paginatedList) 
        => new(paginatedList.TotalCount, paginatedList.PageIndex, paginatedList.PageSize, paginatedList.TotalPages, paginatedList.HasPreviousPage, paginatedList.HasNextPage);
}