using System.Reflection;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Kernel;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using Address = SFA.DAS.Recruit.Api.Models.Address;

namespace SFA.DAS.Recruit.Api.Testing.Data;

public class VacancyEntitySpecimenBuilder : ISpecimenBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not PropertyInfo pi || pi.DeclaringType != typeof(VacancyEntity))
        {
            return new NoSpecimen();
        }

        switch (pi.Name)
        {
            case "EmployerLocations":
                return JsonSerializer.Serialize(context.CreateMany<Address>(), JsonOptions);
            case "Skills":
                return JsonSerializer.Serialize(context.CreateMany<string>(), JsonOptions);
            case "Qualifications":
                return JsonSerializer.Serialize(context.CreateMany<Qualification>(), JsonOptions);
            case "TransferInfo":
                return JsonSerializer.Serialize(context.Create<TransferInfo>(), JsonOptions);
            case "EmployerReviewFieldIndicators":
            case "ProviderReviewFieldIndicators":
                return JsonSerializer.Serialize(context.CreateMany<ReviewFieldIndicator>(), JsonOptions);
            case "TrainingProvider_Address":
                return JsonSerializer.Serialize(context.Create<Address>(), JsonOptions);
            case "ContactPhone":
            case "ProgrammeId":
                return JsonSerializer.Serialize(context.Create<long>(), JsonOptions);
            case "VacancyReference":
                return VacancyReferenceGenerator.GetNextVacancyReference().Value;
            default: return new NoSpecimen();
        }
    }
}