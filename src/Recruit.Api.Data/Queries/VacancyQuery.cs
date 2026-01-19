using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Queries;

[QueryType]
public static class VacancyQuery
{
    [UseProjection, UseFiltering, UseSorting]
    public static IQueryable<VacancyEntity> GetVacancies(GraphQlDataContext dbContext)
    {
        return dbContext.Vacancies.AsNoTracking().AsQueryable();
    }
}