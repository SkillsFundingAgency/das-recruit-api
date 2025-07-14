using System.Reflection;
using AutoFixture.Kernel;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests;

public class VacancyReferenceSpecimenBuilder : ISpecimenBuilder
{
    private readonly Random _random = new();

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo param)
        {
            return new NoSpecimen();
        }
        
        if (param.Member.DeclaringType == typeof(VacancyReference)
            && param.ParameterType == typeof(string)
            && param.Name == "value")
        {
            return $"VAC{_random.Next(100, 9999999)}";
        }
            
        return new NoSpecimen();
    }
}