using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingEmailBaseUrls
{
    [Test]
    [MoqInlineAutoData("prd", "https://recruit.manage-apprenticeships.service.gov.uk")]
    [MoqInlineAutoData("PRD", "https://recruit.manage-apprenticeships.service.gov.uk")]
    [MoqInlineAutoData("local", "https://recruit.local-eas.apprenticeships.education.gov.uk")]
    [MoqInlineAutoData("DEV", "https://recruit.dev-eas.apprenticeships.education.gov.uk")]
    [MoqInlineAutoData("tEsT", "https://recruit.test-eas.apprenticeships.education.gov.uk")]
    public void Then_The_Correct_RecruitEmployerBaseUrl_Is_Returned(string environment, string expectedValue)
    {
        // arrange
        var sut = new EmailTemplateHelper(environment);

        // act
        string result = sut.RecruitEmployerBaseUrl;

        // assert
        result.Should().Be(expectedValue);
    }
}