using System.Linq.Expressions;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

public enum VacancySortColumn
{
    CreatedDate, // first item is default sort column
    Id,
    VacancyReference,
}

public static class VacancySortColumnExtensions
{
    public static Expression<Func<VacancyEntity, object?>> Resolve(this VacancySortColumn enumValue)
    {
        return enumValue switch {
            VacancySortColumn.CreatedDate => x => x.CreatedDate,
            VacancySortColumn.Id => x => x.Id,
            VacancySortColumn.VacancyReference => x => x.VacancyReference,
            _ => throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, null)
        };
    }
} 