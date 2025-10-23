using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.ApplicationReview;

public class WhenGettingApplicationSubmittedNotifications
{
    [Test, RecursiveMoqAutoData]
    public void Then_If_No_Vacancy_Can_Be_Found_An_Exception_Is_Thrown(
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Greedy] ApplicationSubmittedNotificationFactory sut)
    {
        // arrange
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((VacancyEntity?)null);

        // act
        var func = async () => await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        func.Should().ThrowAsync<DataIntegrityException>();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_No_Users_Are_Found_For_A_Provider_Created_Vacancy_Then_No_Notifications_Are_Generated(
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] ApplicationSubmittedNotificationFactory sut)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Provider;
        applicationReview.Vacancy = vacancy;


        long? capturedUkprn = null;
        userRepository
            .Setup(x => x.FindUsersByUkprnAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Callback((long x, CancellationToken _) => capturedUkprn = x)
            .ReturnsAsync([]);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        capturedUkprn.Should().Be(vacancy.Ukprn);
        result.Delayed.Should().BeEmpty();
        result.Immediate.Should().BeEmpty();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_No_Users_Are_Found_For_An_Employer_Created_Vacancy_Then_No_Notifications_Are_Generated(
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] ApplicationSubmittedNotificationFactory sut)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Employer;
        applicationReview.Vacancy = vacancy;


        long? capturedAccountId = null;
        userRepository
            .Setup(x => x.FindUsersByEmployerAccountIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Callback((long x, CancellationToken _) => capturedAccountId = x)
            .ReturnsAsync([]);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        capturedAccountId.Should().Be(applicationReview.AccountId);
        result.Delayed.Should().BeEmpty();
        result.Immediate.Should().BeEmpty();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_No_Users_Wish_To_Receive_Notification_Then_No_Notifications_Are_Generated(
        List<UserEntity> users,
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] ApplicationSubmittedNotificationFactory sut)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Employer;
        users.ForEach(x => x.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Never));
        users[0].SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.UserSubmittedVacancies, NotificationFrequency.Never);
        vacancy.ReviewRequestedByUserId = users[0].Id;
        applicationReview.Vacancy = vacancy;

        userRepository
            .Setup(x => x.FindUsersByEmployerAccountIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        result.Delayed.Should().BeEmpty();
        result.Immediate.Should().BeEmpty();
    }
}