using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.Vacancy;

public class WhenGettingFactory
{
    [Test]
    [RecursiveMoqInlineAutoData(VacancyStatus.Rejected, typeof(VacancyRejectedNotificationFactory))]
    [RecursiveMoqInlineAutoData(VacancyStatus.Review, typeof(VacancySentForReviewNotificationFactory))]
    [RecursiveMoqInlineAutoData(VacancyStatus.Submitted, typeof(VacancySubmittedNotificationFactory))]
    public void Then_The_Correct_Factory_Is_Returned(
        VacancyStatus status,
        Type type,
        VacancyEntity vacancy,
        [Greedy] VacancyNotificationStrategy sut)
    {
        // arrange
        vacancy.Status = status;

        // act
        var result = sut.Create(vacancy);

        // assert
        result.Should().BeOfType(type);
    }
}