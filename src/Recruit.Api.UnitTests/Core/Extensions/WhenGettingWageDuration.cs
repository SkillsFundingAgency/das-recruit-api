using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Extensions;

public class WhenGettingWageDuration
{
    [Test]
    [MoqInlineAutoData(null, null, "" )]
    [MoqInlineAutoData(DurationUnit.Week, 1, "1 week" )]
    [MoqInlineAutoData(DurationUnit.Week, 2, "2 weeks" )]
    [MoqInlineAutoData(DurationUnit.Month, 1, "1 month" )]
    [MoqInlineAutoData(DurationUnit.Month, 2, "2 months" )]
    [MoqInlineAutoData(DurationUnit.Month, 24, "2 years" )]
    [MoqInlineAutoData(DurationUnit.Month, 13, "1 year 1 month" )]
    [MoqInlineAutoData(DurationUnit.Month, 14, "1 year 2 months" )]
    [MoqInlineAutoData(DurationUnit.Month, 26, "2 years 2 months" )]
    [MoqInlineAutoData(DurationUnit.Year, 1, "1 year" )]
    [MoqInlineAutoData(DurationUnit.Year, 2, "2 years" )]
    public void Then_The_Value_Will_Be_Converted_Correctly(DurationUnit? durationUnit, int? duration, string expectedOutput, VacancyEntity vacancy)
    {
        // arrange
        vacancy.Wage_DurationUnit = durationUnit;
        vacancy.Wage_Duration = duration;

        // act
        var result = vacancy.GetWageDuration();

        // assert
        result.Should().Be(expectedOutput);
    }
}