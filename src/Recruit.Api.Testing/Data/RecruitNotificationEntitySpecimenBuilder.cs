using System.Reflection;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Kernel;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Testing.Data;

public class RecruitNotificationEntitySpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not PropertyInfo pi || pi.DeclaringType != typeof(RecruitNotificationEntity))
        {
            return new NoSpecimen();
        }
        
        switch (pi.Name)
        {
            case "User": return context.Create<UserEntity>();
            case "StaticData": return JsonSerializer.Serialize(context.Create<Dictionary<string, string>>());
            case "DynamicData": return JsonSerializer.Serialize(context.Create<Dictionary<string, string>>());
            default: return new NoSpecimen();
        }
    }
}