using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingEmailTemplateId
{
    [Test]
    [MoqInlineAutoData(EmailTemplates.ApplicationReviewShared, "53058846-e369-4396-87b2-015c9d16360a")]
    [MoqInlineAutoData(EmailTemplates.EmployerHasReviewedSharedApplication, "2f1b70d4-c722-4815-85a0-80a080eac642")]
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
    [MoqInlineAutoData(EmailTemplates.EmployerHasReviewedSharedApplication, "feb4191d-a373-4040-9bc6-93c09d8039b5")]
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