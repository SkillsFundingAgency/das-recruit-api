using System.Reflection;
using AutoFixture.Kernel;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.UnitTests;
public class UserEntitySpecimenBuilder : ISpecimenBuilder
{
    private const int RangeInMonths = 11;
    private readonly RandomNumericSequenceGenerator _randomizer = new RandomNumericSequenceGenerator(DateTime.UtcNow.AddMonths(-RangeInMonths).Ticks, DateTime.UtcNow.AddSeconds(-1).Ticks);

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not PropertyInfo pi || pi.DeclaringType != typeof(UserEntity))
        {
            return new NoSpecimen();
        }

        return pi.Name switch {
            "LastSignedInDate" => CreateRandomDate(context),
            _ => new NoSpecimen()
        };
    }

    private object CreateRandomDate(ISpecimenContext context)
    {
        var result = new DateTime(GetRandomNumberOfTicks(context));
        return result.AddTicks(-Convert.ToInt64(result.Ticks % TimeSpan.TicksPerSecond));
    }

    private long GetRandomNumberOfTicks(ISpecimenContext context)
    {
        return (long)_randomizer.Create(typeof(long), context);
    }
}

