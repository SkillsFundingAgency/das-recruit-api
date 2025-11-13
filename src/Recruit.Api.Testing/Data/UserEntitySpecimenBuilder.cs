using System.Reflection;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Kernel;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Testing.Data;

public class UserEntitySpecimenBuilder : ISpecimenBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not PropertyInfo pi || pi.DeclaringType != typeof(UserEntity))
        {
            return new NoSpecimen();
        }

        switch (pi.Name)
        {
            case "NotificationPreferences":
                return JsonSerializer.Serialize(context.Create<NotificationPreferences>(), JsonOptions);
            default: return new NoSpecimen();
        }
    }
}