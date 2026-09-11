using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.FeedbackNudgeEmail;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.VacancyFeedback;

public class WhenGettingVacancyFeedbackNotifications
{
    [Test]
    [MoqInlineAutoData(VacancyStatus.Draft)]
    [MoqInlineAutoData(VacancyStatus.Review)]
    [MoqInlineAutoData(VacancyStatus.Rejected)]
    [MoqInlineAutoData(VacancyStatus.Submitted)]
    [MoqInlineAutoData(VacancyStatus.Referred)]
    [MoqInlineAutoData(VacancyStatus.Live)]
    [MoqInlineAutoData(VacancyStatus.Approved)]
    [MoqInlineAutoData(VacancyStatus.Archived)]
    public async Task Then_No_Notifications_Are_Generated_For_Non_Closed_Vacancies(
        VacancyStatus vacancyStatus,
        VacancyEntity vacancy,
        [Greedy] VacancyFeedbackNotificationFactory sut)
    {
        // arrange
        vacancy.Status = vacancyStatus;
        var data = new Dictionary<string, string> { ["feedbackCount"] = "10" };

        // act
        var result = await sut.CreateAsync(vacancy, data, CancellationToken.None);

        // assert
        result.Immediate.Should().BeEmpty();
    }
    
    [Test]
    [MoqInlineAutoData(OwnerType.Unknown)]
    [MoqInlineAutoData(OwnerType.External)]
    public async Task Then_No_Notifications_Are_Generated_For_Unhandled_Owner_Types(
        OwnerType ownerType,
        VacancyEntity vacancy,
        [Greedy] VacancyFeedbackNotificationFactory sut)
    {
        // arrange
        vacancy.Status = VacancyStatus.Closed;
        vacancy.OwnerType = ownerType;
        var data = new Dictionary<string, string> { ["feedbackCount"] = "10" };

        // act
        var result = await sut.CreateAsync(vacancy, data, CancellationToken.None);

        // assert
        result.Immediate.Should().BeEmpty();
    }
    
    [Test]
    [RecursiveMoqInlineAutoData(OwnerType.Employer)]
    [RecursiveMoqInlineAutoData(OwnerType.Provider)]
    public async Task Then_No_Notifications_Are_Generated_When_The_User_Cannot_Be_Found(
        OwnerType ownerType,
        VacancyEntity vacancyEntity,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] VacancyFeedbackNotificationFactory sut)
    {
        // arrange
        vacancyEntity.Status = VacancyStatus.Closed;
        vacancyEntity.OwnerType = ownerType;
        var data = new Dictionary<string, string> { ["feedbackCount"] = "10" };

        userRepository
            .Setup(x => x.FindByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);
        
        // act
        var result = await sut.CreateAsync(vacancyEntity, data, CancellationToken.None);

        // assert
        result.Immediate.Should().HaveCount(0);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Notifications_Are_Generated_For_An_Employer_Vacancy(
        VacancyEntity vacancyEntity,
        UserEntity userEntity,
        string hashedAccountId,
        string expectedManageAdvertUrl,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEncodingService> encodingService,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancyFeedbackNotificationFactory sut)
    {
        // arrange
        vacancyEntity.Status = VacancyStatus.Closed;
        vacancyEntity.OwnerType = OwnerType.Employer;
        var data = new Dictionary<string, string> { ["feedbackCount"] = "10" };

        userRepository
            .Setup(x => x.FindByUserIdAsync(vacancyEntity.SubmittedByUserId.ToString()!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        encodingService
            .Setup(x => x.Encode(vacancyEntity.AccountId!.Value, EncodingType.AccountId))
            .Returns(hashedAccountId);
        
        emailTemplateHelper
            .Setup(x => x.EmployerManageVacancyUrl(hashedAccountId, vacancyEntity.Id))
            .Returns(expectedManageAdvertUrl);

        // act
        var result = await sut.CreateAsync(vacancyEntity, data, CancellationToken.None);

        // assert
        result.Immediate.Should().HaveCount(1);
        result.Immediate[0].UserId.Should().Be(userEntity.Id);
        result.Immediate[0].User.Should().Be(userEntity);
        result.Immediate[0].SendWhen.Should().BeWithin(TimeSpan.FromSeconds(5));
        result.Immediate[0].EmailTemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequired);
        result.Immediate[0].StaticData.Should().NotBeEmpty();
        result.Immediate[0].DynamicData.Should().NotBeEmpty();
        
        var staticData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(result.Immediate[0].StaticData);
        var dynamicData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(result.Immediate[0].DynamicData);
        staticData.Should().NotBeNull();
        dynamicData.Should().NotBeNull();
        
        staticData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("firstName", userEntity.Name));
        staticData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("feedbackCount", "10"));
        staticData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("advertNoun", "advert"));
        dynamicData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("advertTitle", vacancyEntity.Title!));
        dynamicData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("manageAdvertURL", expectedManageAdvertUrl));
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Notifications_Are_Generated_For_An_Provider_Vacancy(
        VacancyEntity vacancyEntity,
        UserEntity userEntity,
        string expectedManageAdvertUrl,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancyFeedbackNotificationFactory sut)
    {
        // arrange
        vacancyEntity.Status = VacancyStatus.Closed;
        vacancyEntity.OwnerType = OwnerType.Provider;
        vacancyEntity.ReviewRequestedByUserId = null;
        var data = new Dictionary<string, string> { ["feedbackCount"] = "10" };

        userRepository
            .Setup(x => x.FindByUserIdAsync(vacancyEntity.SubmittedByUserId.ToString()!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);
        
        emailTemplateHelper
            .Setup(x => x.ProviderManageVacancyUrl(vacancyEntity.Ukprn!.Value.ToString(), vacancyEntity.Id))
            .Returns(expectedManageAdvertUrl);

        // act
        var result = await sut.CreateAsync(vacancyEntity, data, CancellationToken.None);

        // assert
        result.Immediate.Should().HaveCount(1);
        result.Immediate[0].UserId.Should().Be(userEntity.Id);
        result.Immediate[0].User.Should().Be(userEntity);
        result.Immediate[0].SendWhen.Should().BeWithin(TimeSpan.FromSeconds(5));
        result.Immediate[0].EmailTemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequired);
        result.Immediate[0].StaticData.Should().NotBeEmpty();
        result.Immediate[0].DynamicData.Should().NotBeEmpty();
        
        var staticData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(result.Immediate[0].StaticData);
        var dynamicData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(result.Immediate[0].DynamicData);
        staticData.Should().NotBeNull();
        dynamicData.Should().NotBeNull();
        
        staticData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("firstName", userEntity.Name));
        staticData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("feedbackCount", "10"));
        staticData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("advertNoun", "vacancy"));
        dynamicData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("advertTitle", vacancyEntity.Title!));
        dynamicData.Should().ContainEquivalentOf(new KeyValuePair<string,string>("manageAdvertURL", expectedManageAdvertUrl));
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Notifications_Are_Generated_For_An_Provider_That_Requires_Employer_Approval_Vacancy(
        VacancyEntity vacancyEntity,
        UserEntity userEntity,
        [Frozen] Mock<IUserRepository> userRepository,
        [Greedy] VacancyFeedbackNotificationFactory sut)
    {
        // arrange
        vacancyEntity.Status = VacancyStatus.Closed;
        vacancyEntity.OwnerType = OwnerType.Provider;
        var data = new Dictionary<string, string> { ["feedbackCount"] = "10" };

        userRepository
            .Setup(x => x.FindByUserIdAsync(vacancyEntity.ReviewRequestedByUserId.ToString()!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        // act
        var result = await sut.CreateAsync(vacancyEntity, data, CancellationToken.None);

        // assert
        result.Immediate.Should().HaveCount(1);
        result.Immediate[0].User.Should().Be(userEntity);
    }
}