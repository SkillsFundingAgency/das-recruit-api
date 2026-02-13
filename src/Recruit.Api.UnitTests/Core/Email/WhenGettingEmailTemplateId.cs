using SFA.DAS.Recruit.Api.Core.Email;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingEmailTemplateId
{
    public static object[] ProductionTestCases = {
        new object[] { (ProductionEmailTemplateIds x) => x.ApplicationSharedWithEmployer, new Guid("53058846-e369-4396-87b2-015c9d16360a") },
        new object[] { (ProductionEmailTemplateIds x) => x.ApplicationSubmittedToEmployerDaily, new Guid("1c8c9e72-86c1-4fd1-8020-f4fe354a6e79") },
        new object[] { (ProductionEmailTemplateIds x) => x.ApplicationSubmittedToEmployerImmediate, new Guid("e07a6992-4d17-4167-b526-2ead6fe9ad4d") },
        new object[] { (ProductionEmailTemplateIds x) => x.ApplicationSubmittedToEmployerWeekly, new Guid("68d467ac-339c-42b4-b862-ca06e1cc66e8") },
        new object[] { (ProductionEmailTemplateIds x) => x.ApplicationSubmittedToProviderDaily, new Guid("8e38bbdc-9632-465b-95b7-01523570e517") },
        new object[] { (ProductionEmailTemplateIds x) => x.ApplicationSubmittedToProviderImmediate, new Guid("8b65443f-06b8-4cc9-a83d-5efb847db222") },
        new object[] { (ProductionEmailTemplateIds x) => x.ApplicationSubmittedToProviderWeekly, new Guid("ec70ddac-54c6-4585-adc0-d19bb25b23d9") },
        new object[] { (ProductionEmailTemplateIds x) => x.EmployerVacancyApprovedByDfe, new Guid("d8855c4f-9ce1-4870-93ff-53e609f59a51") },
        new object[] { (ProductionEmailTemplateIds x) => x.EmployerVacancyRejectedByDfe, new Guid("27acd0e9-96fe-47ec-ae33-785e00a453f8") },
        new object[] { (ProductionEmailTemplateIds x) => x.ProviderAttachedToVacancy, new Guid("ce5df943-09ad-4f6c-b10a-42584f88046d") },
        new object[] { (ProductionEmailTemplateIds x) => x.ProviderVacancyApprovedByDfe, new Guid("ee2d7ab3-7ac1-47f8-bc32-86290bda55c9") },
        new object[] { (ProductionEmailTemplateIds x) => x.ProviderVacancyApprovedByEmployer, new Guid("c35e76e7-303b-4b18-bb06-ad98cf68158d") },
        new object[] { (ProductionEmailTemplateIds x) => x.ProviderVacancyRejectedByDfe, new Guid("872e847b-77f5-44a7-b12e-4a19df969ec1") },
        new object[] { (ProductionEmailTemplateIds x) => x.ProviderVacancyRejectedByEmployer, new Guid("8df54598-fea3-45c2-83f3-cca010a6443c") },
        new object[] { (ProductionEmailTemplateIds x) => x.ProviderVacancySentForEmployerReview, new Guid("2b69c0b2-bcc0-4988-82b6-868874e5617b") },
        new object[] { (ProductionEmailTemplateIds x) => x.SharedApplicationReviewedByEmployer, new Guid("2f1b70d4-c722-4815-85a0-80a080eac642") },
    };
    
    [TestCaseSource(nameof(ProductionTestCases))]
    public void Then_The_Production_Template_Ids_Are_Correct(Func<ProductionEmailTemplateIds, Guid> func, Guid expectedGuid)
    {
        // arrange
        var sut = new ProductionEmailTemplateIds();

        // act
        var result = func(sut);

        // assert
        result.Should().Be(expectedGuid);

    }
    
    public static object[] DevelopmentTestCases = {
        new object[] { (DevelopmentEmailTemplateIds x) => x.ApplicationSharedWithEmployer, new Guid("f6fc57e6-7318-473d-8cb5-ca653035391a") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ApplicationSubmittedToEmployerDaily, new Guid("b793a50f-49f0-4b3f-a4c3-46a8f857e48c") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ApplicationSubmittedToEmployerImmediate, new  Guid("8aedd294-fd12-4b77-b4b8-2066744e1fdc") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ApplicationSubmittedToEmployerWeekly, new Guid("520a434a-2203-49f6-a15a-9e9d1c58c18f") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ApplicationSubmittedToProviderDaily, new Guid("f4975bd2-ec66-4f84-a7a6-9693a4f13da3") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ApplicationSubmittedToProviderImmediate, new Guid("d9b4b7f3-59ce-46d2-b477-f283f5ab3084") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ApplicationSubmittedToProviderWeekly, new Guid("95cc2775-b6f2-4824-a4d9-c394fe0e7aff") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.EmployerVacancyApprovedByDfe, new Guid("9a45ff1d-769d-4be2-96fb-dcf605e0108f") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.EmployerVacancyRejectedByDfe, new Guid("5869140a-2a76-4a7c-b4b9-083d2afc5aa5") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ProviderAttachedToVacancy, new Guid("b00a94c3-4b6e-48df-b28b-a768600fe7a5") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ProviderVacancyApprovedByDfe, new Guid("48c9ab9e-5b13-4843-b4d5-ee1caa46cc64") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ProviderVacancyApprovedByEmployer, new Guid("c445095e-e659-499b-b2ab-81e321a9b591") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ProviderVacancyRejectedByDfe, new Guid("048d93c9-4371-45a3-96c4-3f93241a5908") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ProviderVacancyRejectedByEmployer, new Guid("6e663255-c59d-4964-bcbe-b5881a14c530") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.ProviderVacancySentForEmployerReview, new Guid("83f6cede-31c3-4dc9-b2ec-922856ba9bdc") },
        new object[] { (DevelopmentEmailTemplateIds x) => x.SharedApplicationReviewedByEmployer, new Guid("feb4191d-a373-4040-9bc6-93c09d8039b5") },
    };
    
    [TestCaseSource(nameof(DevelopmentTestCases))]
    public void Then_The_Development_Template_Ids_Are_Correct(Func<DevelopmentEmailTemplateIds, Guid> func, Guid expectedGuid)
    {
        // arrange
        var sut = new DevelopmentEmailTemplateIds();

        // act
        var result = func(sut);

        // assert
        result.Should().Be(expectedGuid);
    }
}