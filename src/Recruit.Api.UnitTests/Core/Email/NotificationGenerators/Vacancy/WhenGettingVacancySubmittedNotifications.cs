using System.Text.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Configuration;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.Vacancy;

public class WhenGettingVacancySubmittedNotifications
{
    [Test]
    [MoqInlineAutoData(VacancyStatus.Draft)]
    [MoqInlineAutoData(VacancyStatus.Rejected)]
    [MoqInlineAutoData(VacancyStatus.Referred)]
    [MoqInlineAutoData(VacancyStatus.Live)]
    [MoqInlineAutoData(VacancyStatus.Closed)]
    [MoqInlineAutoData(VacancyStatus.Approved)]
    [MoqInlineAutoData(VacancyStatus.Review)]
    public async Task Vacancy_With_The_Incorrect_Status_Will_Not_Be_Processed(
        VacancyStatus status,
        VacancyEntity vacancy,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancySubmittedNotificationFactory sut)
    {
        // arrange
        vacancy.Status = status;
        vacancy.ReviewRequestedByUserId = Guid.NewGuid();
        vacancy.OwnerType = OwnerType.Provider;
        
        // act
        await sut.CreateAsync(vacancy, CancellationToken.None);

        // assert
        userRepository.Verify(x => x.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test]
    [MoqInlineAutoData(OwnerType.Employer)]
    [MoqInlineAutoData(OwnerType.External)]
    [MoqInlineAutoData(OwnerType.Unknown)]
    public async Task Vacancy_With_The_Incorrect_OwnerType_Will_Not_Be_Processed(
        OwnerType ownerType,
        VacancyEntity vacancy,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancySubmittedNotificationFactory sut)
    {
        // arrange
        vacancy.Status = VacancyStatus.Submitted;
        vacancy.ReviewRequestedByUserId = Guid.NewGuid();
        vacancy.OwnerType = ownerType;
        
        // act
        await sut.CreateAsync(vacancy, CancellationToken.None);

        // assert
        userRepository.Verify(x => x.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test, MoqAutoData]
    public async Task Vacancy_That_Has_Not_Be_Sent_For_Review_Will_Not_Be_Processed(
        VacancyEntity vacancy,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancySubmittedNotificationFactory sut)
    {
        // arrange
        vacancy.Status = VacancyStatus.Submitted;
        vacancy.ReviewRequestedByUserId = null;
        vacancy.OwnerType = OwnerType.Provider;

        // act
        await sut.CreateAsync(vacancy, CancellationToken.None);

        // assert
        userRepository.Verify(x => x.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task The_Notifications_Contain_The_Correct_Values(
        VacancyEntity vacancy,
        UserEntity user,
        string hashedEmployerAccountId,
        Guid templateId,
        string baseUrl,
        string notificationSettingsUrl,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEncodingService> encodingService,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancySubmittedNotificationFactory sut)
    {
        // arrange
        var cts = new CancellationTokenSource();
        
        vacancy.Status = VacancyStatus.Submitted;
        vacancy.ReviewRequestedByUserId = Guid.NewGuid();
        vacancy.OwnerType = OwnerType.Provider;

        userRepository
            .Setup(x => x.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cts.Token))
            .ReturnsAsync([user]);
        emailTemplateHelper
            .Setup(x => x.GetTemplateId(NotificationTypes.VacancyApprovedOrRejected, NotificationFrequency.Immediately))
            .Returns(templateId);
        emailTemplateHelper
            .Setup(x => x.RecruitProviderBaseUrl)
            .Returns(baseUrl);
        emailTemplateHelper
            .Setup(x => x.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()))
            .Returns(notificationSettingsUrl);

        // act
        var results = await sut.CreateAsync(vacancy, cts.Token);

        // assert
        results.Delayed.Should().BeEmpty();
        var notification = results.Immediate.Single();
        notification.User.Should().Be(user);
        notification.UserId.Should().Be(user.Id);
        notification.SendWhen.Should().BeWithin(TimeSpan.FromSeconds(5));
        notification.DynamicData.Should().Be("{}");
        notification.EmailTemplateId.Should().Be(templateId);
        var tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.StaticData, JsonConfig.Options);
        tokens!.Count.Should().Be(6);
        tokens["firstName"].Should().Be(user.Name);
        tokens["advertTitle"].Should().Be(vacancy.Title);
        tokens["VACcode"].Should().Be(new VacancyReference(vacancy.VacancyReference).ToShortString());
        tokens["employerName"].Should().Be(vacancy.EmployerName);
        tokens["location"].Should().Be(vacancy.GetLocationText(JsonConfig.Options));
        tokens["notificationSettingsURL"].Should().Be(notificationSettingsUrl);
    }
}