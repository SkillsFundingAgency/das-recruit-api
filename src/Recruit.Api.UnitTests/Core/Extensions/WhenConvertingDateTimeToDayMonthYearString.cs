using SFA.DAS.Recruit.Api.Core.Extensions;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Extensions;

public class WhenConvertingDateTimeToDayMonthYearString
{
    [Test]
    public void Then_The_Value_Will_Be_Converted_Correctly()
    {
        // arrange
        DateTime? value = new DateTime(2026, 6, 15);

        // act
        var result = value.ToDayMonthYearString();

        // assert
        result.Should().Be("15 June 2026");
    }
    
    [Test]
    public void Then_Null_Values_Return_Empty_String()
    {
        // arrange
        DateTime? value = null;

        // act
        var result = value.ToDayMonthYearString();

        // assert
        result.Should().Be(string.Empty);
    }
}