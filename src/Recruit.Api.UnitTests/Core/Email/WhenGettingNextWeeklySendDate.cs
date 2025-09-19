using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingNextWeeklySendDate
{
    [Test]
    [MoqInlineAutoData(2025, 1, 1, 2025, 1, 6)]
    [MoqInlineAutoData(2024, 2, 29, 2024, 3, 4)]
    [MoqInlineAutoData(2026, 12, 31, 2027, 1, 4)]
    public void Then_The_Next_Monday_Morning_At_12am_Is_Returned(int year, int month, int day, int expectedYear, int expectedMonth, int expectedDay)
    {
        // arrange
        var dateTime = new DateTime(year, month, day);

        // act
        var result = dateTime.GetNextWeeklySendDate();

        // assert
        result.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.Year.Should().Be(expectedYear);
        result.Month.Should().Be(expectedMonth);
        result.Day.Should().Be(expectedDay);
        result.TimeOfDay.Should().Be(new TimeSpan(0, 0, 0, 0));
    }
}