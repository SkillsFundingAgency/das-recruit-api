using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingEmailTemplateId
{
    [Test]
    [MoqInlineAutoData(NotificationTypes.ApplicationSharedWithEmployer, NotificationFrequency.Immediately, "53058846-e369-4396-87b2-015c9d16360a")]
    [MoqInlineAutoData(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationFrequency.Immediately, "2f1b70d4-c722-4815-85a0-80a080eac642")]
    [MoqInlineAutoData(NotificationTypes.VacancySentForReview, NotificationFrequency.Immediately, "2b69c0b2-bcc0-4988-82b6-868874e5617b")]
    [MoqInlineAutoData(NotificationTypes.VacancyApprovedOrRejected, NotificationFrequency.Immediately, "c35e76e7-303b-4b18-bb06-ad98cf68158d")]
    public void Then_The_Correct_Production_Template_Is_Returned(NotificationTypes notificationType, NotificationFrequency frequency, string expectedTemplateId)
    {
        // arrange
        var sut = new EmailTemplateHelper("prd");

        // act
        var result = sut.GetTemplateId(notificationType, frequency);

        // assert
        result.ToString().Should().Be(expectedTemplateId);
    }
    
    [Test]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately, UserType.Employer, "e07a6992-4d17-4167-b526-2ead6fe9ad4d")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily, UserType.Employer, "1c8c9e72-86c1-4fd1-8020-f4fe354a6e79")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly, UserType.Employer, "68d467ac-339c-42b4-b862-ca06e1cc66e8")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately, UserType.Provider, "8b65443f-06b8-4cc9-a83d-5efb847db222")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily, UserType.Provider, "8e38bbdc-9632-465b-95b7-01523570e517")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly, UserType.Provider, "ec70ddac-54c6-4585-adc0-d19bb25b23d9")]
    public void Then_The_Correct_Production_UserType_Specific_Template_Is_Returned(NotificationTypes notificationType, NotificationFrequency frequency, UserType userType, string expectedTemplateId)
    {
        // arrange
        var sut = new EmailTemplateHelper("prd");

        // act
        var result = sut.GetTemplateId(notificationType, frequency, userType);

        // assert
        result.ToString().Should().Be(expectedTemplateId);
    }
    
    [Test]
    [MoqInlineAutoData(NotificationTypes.ApplicationSharedWithEmployer, NotificationFrequency.Immediately, "f6fc57e6-7318-473d-8cb5-ca653035391a")]
    [MoqInlineAutoData(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationFrequency.Immediately, "feb4191d-a373-4040-9bc6-93c09d8039b5")]
    [MoqInlineAutoData(NotificationTypes.VacancySentForReview, NotificationFrequency.Immediately, "83f6cede-31c3-4dc9-b2ec-922856ba9bdc")]
    [MoqInlineAutoData(NotificationTypes.VacancyApprovedOrRejected, NotificationFrequency.Immediately, "c445095e-e659-499b-b2ab-81e321a9b591")]
    public void Then_The_Correct_Development_Template_Is_Returned(NotificationTypes notificationType, NotificationFrequency frequency, string expectedTemplateId)
    {
        // arrange
        var sut = new EmailTemplateHelper("local");

        // act
        var result = sut.GetTemplateId(notificationType, frequency);

        // assert
        result.ToString().Should().Be(expectedTemplateId);
    }
    
    [Test]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately, UserType.Employer, "8aedd294-fd12-4b77-b4b8-2066744e1fdc")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily, UserType.Employer, "b793a50f-49f0-4b3f-a4c3-46a8f857e48c")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly, UserType.Employer, "520a434a-2203-49f6-a15a-9e9d1c58c18f")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately, UserType.Provider, "d9b4b7f3-59ce-46d2-b477-f283f5ab3084")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily, UserType.Provider, "f4975bd2-ec66-4f84-a7a6-9693a4f13da3")]
    [MoqInlineAutoData(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly, UserType.Provider, "95cc2775-b6f2-4824-a4d9-c394fe0e7aff")]
    public void Then_The_Correct_Development_UserType_Specific_Template_Is_Returned(NotificationTypes notificationType, NotificationFrequency frequency, UserType userType, string expectedTemplateId)
    {
        // arrange
        var sut = new EmailTemplateHelper("local");

        // act
        var result = sut.GetTemplateId(notificationType, frequency, userType);

        // assert
        result.ToString().Should().Be(expectedTemplateId);
    }
}