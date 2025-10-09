using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingNextDailySendDate
{
    [Test, MoqAutoData]
    public void Then_The_Date_Returned_Is_Tomorrow_Morning_At_12_Am()
    {
        // arrange
        DateTime now = DateTime.Now;

        // act
        var result = now.GetNextDailySendDate();

        // assert
        result.Should().Be(now.AddDays(1).Date);
    }
}