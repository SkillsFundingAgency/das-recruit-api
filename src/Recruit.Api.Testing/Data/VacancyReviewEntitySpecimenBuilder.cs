using System.Reflection;
using AutoFixture.Kernel;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Testing.Data;

public class VacancyReviewEntitySpecimenBuilder : ISpecimenBuilder
{
    private readonly Random _random = new();

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not PropertyInfo pi || pi.DeclaringType != typeof(VacancyReviewEntity))
        {
            return new NoSpecimen();
        }

        switch (pi.Name)
        {
            case "ManualQaFieldIndicators":
            case "UpdatedFieldIdentifiers":
            case "DismissedAutomatedQaOutcomeIndicators":
                var range = Enumerable.Range(0, _random.Next(0, 10));
                var items = range.Select(_ => $"\"{Guid.NewGuid().ToString()}\"").ToList();
                return items.Count > 0
                    ? $"[{string.Join(",", items)}]"
                    : "[]";
            default: return new NoSpecimen();
        }
    }
}