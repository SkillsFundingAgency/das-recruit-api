using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.Vacancy;

internal class WhenGettingFactory
{
    [Test, RecursiveMoqAutoData]
    public void Then_The_Vacancy_Submitted_Factory_Is_Returned_For_Submitted_Vacancies(
        VacancyEntity vacancy,
        [Greedy] VacancyNotificationStrategy sut)
    {
        // arrange
        vacancy.Status = VacancyStatus.Submitted;

        // act
        var result = sut.Create(vacancy);

        // assert
        result.Should().BeOfType<VacancySubmittedNotificationFactory>();
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_The_Vacancy_Sent_For_Review_Factory_Is_Returned_For_Review_Vacancies(
        VacancyEntity vacancy,
        [Greedy] VacancyNotificationStrategy sut)
    {
        // arrange
        vacancy.Status = VacancyStatus.Review;

        // act
        var result = sut.Create(vacancy);

        // assert
        result.Should().BeOfType<VacancySentForReviewNotificationFactory>();
    }
}