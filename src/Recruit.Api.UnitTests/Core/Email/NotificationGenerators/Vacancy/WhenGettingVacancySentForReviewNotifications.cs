using System.Text.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.Vacancy;

public class WhenGettingVacancySentForReviewNotifications
{
    [Test]
    [RecursiveMoqInlineAutoData(VacancyStatus.Draft)]
    [RecursiveMoqInlineAutoData(VacancyStatus.Rejected)]
    [RecursiveMoqInlineAutoData(VacancyStatus.Submitted)]
    [RecursiveMoqInlineAutoData(VacancyStatus.Referred)]
    [RecursiveMoqInlineAutoData(VacancyStatus.Live)]
    [RecursiveMoqInlineAutoData(VacancyStatus.Closed)]
    [RecursiveMoqInlineAutoData(VacancyStatus.Approved)]
    public async Task Vacancy_With_The_Incorrect_Status_Will_Not_Be_Processed(
        VacancyStatus status,
        VacancyEntity vacancy,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancySentForReviewNotificationFactory sut)
    {
        // arrange
        vacancy.Status = status;
        vacancy.ReviewRequestedByUserId = Guid.NewGuid();
        vacancy.OwnerType = OwnerType.Provider;
        
        // act
        await sut.CreateAsync(vacancy, CancellationToken.None);

        // assert
        userRepository.Verify(x => x.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test]
    [RecursiveMoqInlineAutoData(OwnerType.Employer)]
    [RecursiveMoqInlineAutoData(OwnerType.External)]
    [RecursiveMoqInlineAutoData(OwnerType.Unknown)]
    public async Task Vacancy_With_The_Incorrect_OwnerType_Will_Not_Be_Processed(
        OwnerType ownerType,
        VacancyEntity vacancy,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancySentForReviewNotificationFactory sut)
    {
        // arrange
        vacancy.Status = VacancyStatus.Referred;
        vacancy.ReviewRequestedByUserId = Guid.NewGuid();
        vacancy.OwnerType = ownerType;
        
        // act
        await sut.CreateAsync(vacancy, CancellationToken.None);

        // assert
        userRepository.Verify(x => x.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Vacancy_That_Has_Not_Be_Sent_For_Review_Will_Not_Be_Processed(
        VacancyEntity vacancy,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancySentForReviewNotificationFactory sut)
    {
        // arrange
        vacancy.Status = VacancyStatus.Referred;
        vacancy.ReviewRequestedByUserId = null;
        vacancy.OwnerType = OwnerType.Provider;

        // act
        await sut.CreateAsync(vacancy, CancellationToken.None);

        // assert
        userRepository.Verify(x => x.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task The_Notifications_Contain_The_Correct_Values(
        VacancyEntity vacancy,
        UserEntity user,
        string hashedEmployerAccountId,
        string baseUrl,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEncodingService> encodingService,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancySentForReviewNotificationFactory sut)
    {
        // arrange
        var cts = new CancellationTokenSource();
        
        vacancy.Status = VacancyStatus.Review;
        vacancy.ReviewRequestedByUserId = Guid.NewGuid();
        vacancy.OwnerType = OwnerType.Provider;

        userRepository
            .Setup(x => x.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, cts.Token))
            .ReturnsAsync([user]);
        encodingService
            .Setup(x => x.Encode(vacancy.AccountId!.Value, EncodingType.AccountId))
            .Returns(hashedEmployerAccountId);
        emailTemplateHelper
            .Setup(x => x.RecruitEmployerBaseUrl)
            .Returns(baseUrl);

        // act
        var results = await sut.CreateAsync(vacancy, cts.Token);

        // assert
        results.Delayed.Should().BeEmpty();
        var notification = results.Immediate.Single();
        notification.User.Should().Be(user);
        notification.UserId.Should().Be(user.Id);
        notification.SendWhen.Should().BeWithin(TimeSpan.FromSeconds(5));
        notification.DynamicData.Should().Be("{}");
        notification.EmailTemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.ProviderVacancySentForEmployerReview);
        var tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.StaticData, JsonConfig.Options);
        tokens!.Count.Should().Be(7);
        tokens["firstName"].Should().Be(user.Name);
        tokens["trainingProviderName"].Should().Be(vacancy.TrainingProvider_Name);
        tokens["advertTitle"].Should().Be(vacancy.Title);
        tokens["vacancyReference"].Should().Be(new VacancyReference(vacancy.VacancyReference).ToShortString());
        tokens["employerName"].Should().Be(vacancy.EmployerName);
        tokens["location"].Should().Be(vacancy.GetLocationText(JsonConfig.Options));
        tokens["reviewAdvertURL"].Should().Be($"{baseUrl}/accounts/{hashedEmployerAccountId}/vacancies/{vacancy.Id}/check-answers");
    }
}