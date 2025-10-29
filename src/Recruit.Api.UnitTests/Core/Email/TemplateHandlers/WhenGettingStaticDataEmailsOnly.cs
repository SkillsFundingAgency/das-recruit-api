using System.Text.Json;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.TemplateHandlers;

public class WhenGettingStaticDataEmailsOnly
{
    [Test, MoqAutoData]
    public void It_Handles_The_Correct_Templates(Mock<IEmailTemplateHelper> emailTemplateHelper, IEmailTemplateIds emailTemplateIds)
    {
        // arrange
        emailTemplateHelper.Setup(x => x.TemplateIds).Returns(emailTemplateIds);
        
        // act
        var sut = new StaticDataEmailHandler(emailTemplateHelper.Object);
        
        // assert
        sut.CanHandle(emailTemplateIds.ApplicationSharedWithEmployer).Should().BeTrue();
        sut.CanHandle(emailTemplateIds.SharedApplicationReviewedByEmployer).Should().BeTrue();
        sut.CanHandle(emailTemplateIds.ProviderVacancySentForEmployerReview).Should().BeTrue();
        sut.CanHandle(emailTemplateIds.ProviderVacancyApprovedByEmployer).Should().BeTrue();
        sut.CanHandle(emailTemplateIds.ApplicationSubmittedToEmployerImmediate).Should().BeTrue();
        sut.CanHandle(emailTemplateIds.ApplicationSubmittedToProviderImmediate).Should().BeTrue();
    }

    [Test, RecruitAutoData]
    public void Notifications_Are_Mapped_To_Emails_Correctly(
        RecruitNotificationEntity notification,
        [Greedy] StaticDataEmailHandler sut)
    {
        // arrange
        var expectedTokens = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.StaticData)!.ToList();

        // act
        var results = sut.CreateNotificationEmails([notification]).ToList();

        // assert
        results.Should().HaveCount(1);
        var email = results[0];
        email.TemplateId.Should().Be(notification.EmailTemplateId);
        email.RecipientAddress.Should().Be(notification.User.Email);
        email.Tokens.Should().HaveCount(expectedTokens.Count);
        email.Tokens.Should().BeEquivalentTo(expectedTokens);
    }
}