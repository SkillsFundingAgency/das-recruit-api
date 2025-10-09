using System.Text.Json;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.TemplateHandlers;

public class WhenGettingStaticDataEmailsOnly
{
    [Test, MoqAutoData]
    public void It_Handles_The_Correct_Templates(Mock<IEmailTemplateHelper> emailTemplateHelper)
    {
        // arrange/act
        new StaticDataEmailHandler(emailTemplateHelper.Object);
        
        // assert
        emailTemplateHelper.Verify(x => x.GetTemplateId(It.IsAny<NotificationTypes>(), It.IsAny<NotificationFrequency>()), Times.Exactly(3));
        
        emailTemplateHelper.Verify(x => x.GetTemplateId(NotificationTypes.ApplicationSharedWithEmployer, NotificationFrequency.Immediately), Times.Once);
        emailTemplateHelper.Verify(x => x.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately), Times.Once);
        emailTemplateHelper.Verify(x => x.GetTemplateId(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationFrequency.Immediately), Times.Once);
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