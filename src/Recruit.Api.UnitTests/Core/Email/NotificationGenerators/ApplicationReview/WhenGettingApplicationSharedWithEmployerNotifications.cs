using System.Text.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.ApplicationReview;

public class WhenGettingApplicationSharedWithEmployerNotifications
{
    [Test, RecursiveMoqAutoData]
    public void Then_If_No_Vacancy_Can_Be_Found_An_Exception_Is_Thrown(
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Greedy] ApplicationSharedWithEmployerNotificationFactory sut)
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
    public async Task Then_If_The_Vacancy_Is_Not_Of_The_Correct_Owner_Type_No_Results_Are_Returned(
        UserEntity user,
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        Guid templateId,
        string manageNotificationsUrl,
        string baseUrl,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] ApplicationSharedWithEmployerNotificationFactory sut)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Employer;
        user.UserType = UserType.Provider;
        user.SetEmailPref(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancy);
        userRepository
            .Setup(x => x.FindUsersByUkprnAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([user]);
        emailTemplateHelper
            .Setup(x => x.GetTemplateId(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationFrequency.Immediately))
            .Returns(templateId);
        emailTemplateHelper
            .Setup(x => x.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()))
            .Returns(manageNotificationsUrl);
        emailTemplateHelper
            .Setup(x => x.RecruitProviderBaseUrl)
            .Returns(baseUrl);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        result.Delayed.Should().BeEmpty();
        result.Immediate.Should().BeEmpty();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_No_Users_Are_Found_No_Notifications_Are_Generated(
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] ApplicationSharedWithEmployerNotificationFactory sut)
    {
        // arrange
        long? capturedVacancyId = null;
        vacancy.OwnerType = OwnerType.Provider;
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Callback((long x, CancellationToken _) => capturedVacancyId = x)
            .ReturnsAsync(vacancy);

        long? capturedAccountId = null;
        userRepository
            .Setup(x => x.FindUsersByEmployerAccountIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Callback((long x, CancellationToken _) => capturedAccountId = x)
            .ReturnsAsync([]);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        capturedVacancyId.Should().Be(applicationReview.VacancyReference);
        capturedAccountId.Should().Be(applicationReview.AccountId);
        result.Delayed.Should().BeEmpty();
        result.Immediate.Should().BeEmpty();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Results_Are_Returned_Regardless_Of_Any_Notification_Settings(
        List<UserEntity> users,
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] ApplicationSharedWithEmployerNotificationFactory sut)
    {
        // arrange
        users.ForEach(x => x.NotificationPreferences = null); // should cause an exception if anything accesses it
        vacancy.OwnerType = OwnerType.Provider;
        long? capturedVacancyId = null;
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Callback((long x, CancellationToken _) => capturedVacancyId = x)
            .ReturnsAsync(vacancy);

        long? capturedAccountId = null;
        userRepository
            .Setup(x => x.FindUsersByEmployerAccountIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Callback((long x, CancellationToken _) => capturedAccountId = x)
            .ReturnsAsync(users);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        capturedVacancyId.Should().Be(applicationReview.VacancyReference);
        capturedAccountId.Should().Be(applicationReview.AccountId);
        result.Delayed.Should().BeEmpty();
        result.Immediate.Should().HaveCount(users.Count);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Result_Should_Have_The_Required_Information(
        UserEntity user,
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        string hashedAccountId,
        string baseUrl,
        [Frozen] Mock<IEncodingService> encodingService,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] ApplicationSharedWithEmployerNotificationFactory sut)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Provider;
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancy);
        userRepository
            .Setup(x => x.FindUsersByEmployerAccountIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([user]);
        encodingService
            .Setup(x => x.Encode(applicationReview.AccountId, EncodingType.AccountId))
            .Returns(hashedAccountId);
        emailTemplateHelper
            .Setup(x => x.RecruitEmployerBaseUrl)
            .Returns(baseUrl);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        result.Delayed.Should().BeEmpty();
        result.Immediate.Should().HaveCount(1);
        
        var notification = result.Immediate[0];
        notification.UserId.Should().Be(user.Id);
        notification.EmailTemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.ApplicationSharedWithEmployer);
        notification.DynamicData.Should().Be("{}");
        
        var tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.StaticData)!;
        tokens.Should().HaveCount(5);
        tokens["firstName"].Should().Be(user.Name);
        tokens["trainingProvider"].Should().Be(vacancy.TrainingProvider_Name);
        tokens["advertTitle"].Should().Be(vacancy.Title);
        tokens["vacancyReference"].Should().Be(new VacancyReference(applicationReview.VacancyReference).ToShortString());
        tokens["applicationUrl"].Should().Be($"{baseUrl}/accounts/{hashedAccountId}/vacancies/{vacancy.Id}/applications/{applicationReview.ApplicationId}/?vacancySharedByProvider=True");
    }
}