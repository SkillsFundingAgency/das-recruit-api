using System.Reflection;
using System.Text.Json;
using AutoFixture.Kernel;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests;
public class ReportEntitySpecimenBuilder : ISpecimenBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not PropertyInfo pi || pi.DeclaringType != typeof(ReportEntity))
        {
            return new NoSpecimen();
        }

        switch (pi.Name)
        {
            case "DynamicCriteria":
                return JsonSerializer.Serialize(context.CreateMany<ReportCriteria>(), JsonOptions);
            default: return new NoSpecimen();
        }
    }
}
