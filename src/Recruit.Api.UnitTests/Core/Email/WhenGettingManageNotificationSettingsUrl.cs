using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingManageNotificationSettingsUrl
{
    [Test, MoqAutoData]
    public void Then_The_Employer_Url_Is_Correct(
        [Frozen] IRecruitBaseUrls recruitBaseUrls,
        EmailTemplateHelper sut)
    {
        // act
        string url = sut.EmployerManageNotificationsUrl("ABCD");

        // assert
        url.Should().Be($"{recruitBaseUrls.RecruitEmployerBaseUrl}/accounts/ABCD/notifications-manage");
    }
    
    [Test, MoqAutoData]
    public void Then_The_Provider_Url_Is_Correct(
        [Frozen] IRecruitBaseUrls recruitBaseUrls,
        EmailTemplateHelper sut)
    {
        // act
        string url = sut.ProviderManageNotificationsUrl("ABCD");

        // assert
        url.Should().Be($"{recruitBaseUrls.RecruitProviderBaseUrl}/ABCD/notifications-manage");
    }
}