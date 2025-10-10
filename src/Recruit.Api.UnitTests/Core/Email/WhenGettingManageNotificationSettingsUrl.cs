using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingManageNotificationSettingsUrl
{
    [Test]
    [MoqInlineAutoData("PRD", "https://recruit.manage-apprenticeships.service.gov.uk/accounts/ABCD/notifications-manage")]
    [MoqInlineAutoData("LOCAL", "https://recruit.local-eas.apprenticeships.education.gov.uk/accounts/ABCD/notifications-manage")]
    public void Then_The_Employer_Url_Is_Correct(string env, string expectedUrl)
    {
        // arrange
        var sut = new EmailTemplateHelper(env);

        // act
        string url = sut.EmployerManageNotificationsUrl("ABCD");

        // assert
        url.Should().Be(expectedUrl);
    }
    
    [Test]
    [MoqInlineAutoData("PRD", "https://recruit.providers.apprenticeships.education.gov.uk/ABCD/notifications-manage")]
    [MoqInlineAutoData("LOCAL", "https://recruit.local-pas.apprenticeships.education.gov.uk/ABCD/notifications-manage")]
    public void Then_The_Provider_Url_Is_Correct(string env, string expectedUrl)
    {
        // arrange
        var sut = new EmailTemplateHelper(env);

        // act
        string url = sut.ProviderManageNotificationsUrl("ABCD");

        // assert
        url.Should().Be(expectedUrl);
    }
}