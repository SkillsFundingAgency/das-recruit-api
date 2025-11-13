using System.Reflection;
using AutoFixture.Kernel;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Testing.Data;

public class VacancyReferenceSpecimenBuilder : ISpecimenBuilder
{
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
            return VacancyReferenceGenerator.GetNext().ToString();
        }
            
        return new NoSpecimen();
    }
}