using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingEmailBaseUrls
{
    [Test, MoqAutoData]
    public void Then_The_Correct_Employer_Recruit_Base_Url_Is_Returned(
        [Frozen] IRecruitBaseUrls recruitBaseUrls,
        EmailTemplateHelper sut)
    {
        // assert
        sut.RecruitEmployerBaseUrl.Should().Be(recruitBaseUrls.RecruitEmployerBaseUrl);
    }
    
    [Test, MoqAutoData]
    public void Then_The_Correct_Provider_Recruit_Base_Url_Is_Returned(
        [Frozen] IRecruitBaseUrls recruitBaseUrls,
        EmailTemplateHelper sut)
    {
        // assert
        sut.RecruitProviderBaseUrl.Should().Be(recruitBaseUrls.RecruitProviderBaseUrl);
    }
}