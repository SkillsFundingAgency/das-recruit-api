using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingEmailTemplateId
{
    [Test]
    [MoqInlineAutoData(EmailTemplates.ApplicationReviewShared, "53058846-e369-4396-87b2-015c9d16360a")]
    public void Then_The_Correct_Production_Template_Is_Returned(EmailTemplates emailTemplate, string expectedTemplateId)
    {
        // arrange
        var sut = new EmailTemplateHelper("prd");

        // act
        var result = sut.GetTemplateId(emailTemplate);

        // assert
        result.ToString().Should().Be(expectedTemplateId);
    }
    
    [Test]
    [MoqInlineAutoData(EmailTemplates.ApplicationReviewShared, "f6fc57e6-7318-473d-8cb5-ca653035391a")]
    public void Then_The_Correct_Development_Template_Is_Returned(EmailTemplates emailTemplate, string expectedTemplateId)
    {
        // arrange
        var sut = new EmailTemplateHelper("local");

        // act
        var result = sut.GetTemplateId(emailTemplate);

        // assert
        result.ToString().Should().Be(expectedTemplateId);
    }
}