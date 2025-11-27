using System.Text.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.ApplicationReview;

public class WhenGettingApplicationSubmittedWeekyNotifications
{
    [Test, RecruitAutoData]
    public async Task Then_Weekly_Notifications_Should_Have_The_Required_Information_For_Provider_Created_Vacancy(
        UserEntity user,
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        string manageNotificationsUrl,
        string manageVacancyUrl,
        string baseUrl,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] ApplicationSubmittedNotificationFactory sut)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        user.UserType = UserType.Provider;
        user.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Weekly);
        
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
        result.Delayed.Should().HaveCount(1);
        result.Immediate.Should().BeEmpty();

        var notification = result.Delayed[0];
        notification.UserId.Should().Be(user.Id);
        notification.EmailTemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.ApplicationSubmittedToProviderWeekly);
        notification.SendWhen.Should().Be(DateTime.Now.GetNextWeeklySendDate());
        
        var tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.StaticData)!;
        tokens.Should().HaveCount(2);
        tokens["firstName"].Should().Be(user.Name);
        tokens["notificationSettingsURL"].Should().Be(manageNotificationsUrl);
        
        tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.DynamicData)!;
        tokens.Should().HaveCount(5);
        tokens["advertTitle"].Should().Be(vacancy.Title);
        tokens["employerName"].Should().Be(vacancy.EmployerName);
        tokens["vacancyReference"].Should().Be(new VacancyReference(applicationReview.VacancyReference).ToShortString());
        tokens["manageAdvertURL"].Should().Be(manageVacancyUrl);
        tokens["location"].Should().Be("Recruiting nationally");
    }
    
    [Test, RecruitAutoData]
    public async Task Then_Weekly_Notifications_Should_Have_The_Required_Information_For_Employer_Created_Vacancy(
        UserEntity user,
        VacancyEntity vacancy,
        ApplicationReviewEntity applicationReview,
        string hashedEmployerAccountId,
        string manageNotificationsUrl,
        string manageVacancyUrl,
        string baseUrl,
        [Frozen] Mock<IEncodingService> encodingService,
        [Frozen] Mock<IVacancyRepository> vacancyRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] ApplicationSubmittedNotificationFactory sut)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Employer;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        user.UserType = UserType.Employer;
        user.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Weekly);
        
        vacancyRepository
            .Setup(x => x.GetOneByVacancyReferenceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancy);
        userRepository
            .Setup(x => x.FindUsersByEmployerAccountIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([user]);
        emailTemplateHelper
            .Setup(x => x.EmployerManageNotificationsUrl(hashedEmployerAccountId))
            .Returns(manageNotificationsUrl);
        emailTemplateHelper
            .Setup(x => x.EmployerManageVacancyUrl(hashedEmployerAccountId, vacancy.Id))
            .Returns(manageVacancyUrl);
        emailTemplateHelper
            .Setup(x => x.RecruitEmployerBaseUrl)
            .Returns(baseUrl);
        encodingService
            .Setup(x => x.Encode(applicationReview.AccountId, EncodingType.AccountId))
            .Returns(hashedEmployerAccountId);

        // act
        var result = await sut.CreateAsync(applicationReview, CancellationToken.None);

        // assert
        result.Delayed.Should().HaveCount(1);
        result.Immediate.Should().BeEmpty();

        var notification = result.Delayed[0];
        notification.UserId.Should().Be(user.Id);
        notification.EmailTemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.ApplicationSubmittedToEmployerWeekly);
        notification.SendWhen.Should().Be(DateTime.Now.GetNextWeeklySendDate());
        
        var tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.StaticData)!;
        tokens.Should().HaveCount(2);
        tokens["firstName"].Should().Be(user.Name);
        tokens["notificationSettingsURL"].Should().Be(manageNotificationsUrl);
        
        tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.DynamicData)!;
        tokens.Should().HaveCount(5);
        tokens["advertTitle"].Should().Be(vacancy.Title);
        tokens["employerName"].Should().Be(vacancy.EmployerName);
        tokens["vacancyReference"].Should().Be(new VacancyReference(applicationReview.VacancyReference).ToShortString());
        tokens["manageAdvertURL"].Should().Be(manageVacancyUrl);
        tokens["location"].Should().Be("Recruiting nationally");
    }
}