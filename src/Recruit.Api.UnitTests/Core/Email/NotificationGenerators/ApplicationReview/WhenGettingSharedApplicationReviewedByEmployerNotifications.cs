using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Testing;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.ApplicationReview;

public class WhenGettingSharedApplicationReviewedByEmployerNotifications
{
    [Test, RecursiveMoqAutoData]
    public void Then_When_No_Vacancy_Can_Be_Found_An_Exception_Is_Thrown(
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Greedy] SharedApplicationReviewedByEmployerNotificationFactory sut)
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
        [Greedy] SharedApplicationReviewedByEmployerNotificationFactory sut)
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
    public async Task Then_When_No_Users_Are_Found_No_Notifications_Are_Generated(
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] SharedApplicationReviewedByEmployerNotificationFactory sut)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Provider;
        long? capturedVacancyId = null;
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Callback((long x, CancellationToken _) => capturedVacancyId = x)
            .ReturnsAsync(vacancy);

        long? capturedUkprn = null;
        userRepository
            .Setup(x => x.FindUsersByUkprnAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Callback((long x, CancellationToken _) => capturedUkprn = x)
            .ReturnsAsync([]);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        capturedVacancyId.Should().Be(applicationReview.VacancyReference);
        capturedUkprn.Should().Be(vacancy.Ukprn);
        result.Delayed.Should().BeEmpty();
        result.Immediate.Should().BeEmpty();
    }
    
    [Test, RecruitAutoData]
    public async Task Then_The_Result_Should_Have_The_Required_Information(
        UserEntity user,
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        string manageNotificationsUrl,
        string manageVacancyUrl,
        string baseUrl,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] SharedApplicationReviewedByEmployerNotificationFactory sut)
    {
        // arrange
        user.UserType = UserType.Provider;
        user.SetEmailPref(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        vacancy.OwnerType = OwnerType.Provider;
        
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancy);
        userRepository
            .Setup(x => x.FindUsersByUkprnAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([user]);
        emailTemplateHelper
            .Setup(x => x.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()))
            .Returns(manageNotificationsUrl);
        emailTemplateHelper
            .Setup(x => x.ProviderManageVacancyUrl(vacancy.Ukprn!.Value.ToString(), vacancy.Id))
            .Returns(manageVacancyUrl);
        emailTemplateHelper
            .Setup(x => x.RecruitProviderBaseUrl)
            .Returns(baseUrl);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        result.Immediate.Should().BeEmpty();
        
        var notification = result.Delayed.Single();
        notification.UserId.Should().Be(user.Id);
        notification.EmailTemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.SharedApplicationReviewedByEmployer);
        notification.DynamicData.Should().Be("{}");
        
        var tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.StaticData)!;
        tokens.Should().HaveCount(6);
        tokens["firstName"].Should().Be(user.Name);
        tokens["employerName"].Should().Be(vacancy.EmployerName);
        tokens["advertTitle"].Should().Be(vacancy.Title);
        tokens["vacancyReference"].Should().Be(new VacancyReference(applicationReview.VacancyReference).ToShortString());
        tokens["manageAdvertURL"].Should().Be(manageVacancyUrl);
        tokens["notificationSettingsURL"].Should().Be(manageNotificationsUrl);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Only_Uses_Who_Wish_To_Receive_Notifications_Should_Have_Them_Generated(
        [MinLength(4), MaxLength(4)] UserEntity[] users,
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] SharedApplicationReviewedByEmployerNotificationFactory sut)
    {
        // arrange
        // these users should receive notifications
        foreach (UserEntity userEntity in users)
        {
            userEntity.LastSignedInDate = DateTime.UtcNow.AddDays(-1);
        }
        users[0].SetEmailPref(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        users[1].SetEmailPref(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationScope.UserSubmittedVacancies, NotificationFrequency.Immediately);
        
        // these users should not receive notifications
        users[2].SetEmailPref(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationScope.OrganisationVacancies, NotificationFrequency.Never);
        // this user does not match being the 'submitter'
        users[3].SetEmailPref(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationScope.UserSubmittedVacancies, NotificationFrequency.Immediately);
        
        vacancy.ReviewRequestedByUserId = users[1].Id;
        vacancy.OwnerType = OwnerType.Provider;

        var expectedIds = users.Take(2).Select(x => x.Id).ToList();
        var unexpectedIds = users.Skip(2).Take(2).Select(x => x.Id).ToList();
        
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancy);
        userRepository
            .Setup(x => x.FindUsersByUkprnAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users.ToList());

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        result.Delayed.Should().HaveCount(2);
        result.Delayed.Should().AllSatisfy(x =>
        {
            expectedIds.Should().Contain(x.UserId);
            unexpectedIds.Should().NotContain(x.UserId);
        });
    }
}