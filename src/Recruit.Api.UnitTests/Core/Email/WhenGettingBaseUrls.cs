using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingBaseUrls
{
    [Test]
    public void Then_The_Production_Urls_Are_Correct()
    {
        // arrange/act
        var sut = new ProductionRecruitBaseUrls();

        // assert
        sut.RecruitEmployerBaseUrl.Should().Be("https://recruit.manage-apprenticeships.service.gov.uk");
        sut.RecruitProviderBaseUrl.Should().Be("https://recruit.providers.apprenticeships.education.gov.uk");
    }
    
    [Test]
    [MoqInlineAutoData("dev")]
    [MoqInlineAutoData("LOCAL")]
    public void The_The_Developer_Urls_Are_Correct(string environmentName)
    {
        // arrange/act
        var sut = new DevelopmentRecruitBaseUrls(environmentName);

        // assert
        sut.RecruitEmployerBaseUrl.Should().Be($"https://recruit.{environmentName.ToLower()}-eas.apprenticeships.education.gov.uk");
        sut.RecruitProviderBaseUrl.Should().Be($"https://recruit.{environmentName.ToLower()}-pas.apprenticeships.education.gov.uk");
    }
}