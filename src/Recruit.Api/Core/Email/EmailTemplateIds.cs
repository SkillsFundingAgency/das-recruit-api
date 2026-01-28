namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IEmailTemplateIds
{
    Guid ApplicationSharedWithEmployer { get; }
    Guid ApplicationSubmittedToEmployerDaily { get; }
    Guid ApplicationSubmittedToEmployerImmediate { get; }
    Guid ApplicationSubmittedToEmployerWeekly { get; }
    Guid ApplicationSubmittedToProviderDaily { get; }
    Guid ApplicationSubmittedToProviderImmediate { get; }
    Guid ApplicationSubmittedToProviderWeekly { get; }
    Guid EmployerVacancyApprovedByDfe { get; }
    Guid EmployerVacancyRejectedByDfe { get; }
    Guid ProviderAttachedToVacancy { get; }
    Guid ProviderVacancyApprovedByDfe { get; }
    Guid ProviderVacancyApprovedByEmployer { get; }
    Guid ProviderVacancyRejectedByDfe { get; }
    Guid ProviderVacancyRejectedByEmployer { get; }
    Guid ProviderVacancySentForEmployerReview { get; }
    Guid SharedApplicationReviewedByEmployer { get; }
}

public class ProductionEmailTemplateIds : IEmailTemplateIds
{
    public Guid ApplicationSharedWithEmployer => new("53058846-e369-4396-87b2-015c9d16360a");
    public Guid ApplicationSubmittedToEmployerDaily => new("1c8c9e72-86c1-4fd1-8020-f4fe354a6e79");
    public Guid ApplicationSubmittedToEmployerImmediate => new("e07a6992-4d17-4167-b526-2ead6fe9ad4d");
    public Guid ApplicationSubmittedToEmployerWeekly => new ("68d467ac-339c-42b4-b862-ca06e1cc66e8");
    public Guid ApplicationSubmittedToProviderDaily => new("8e38bbdc-9632-465b-95b7-01523570e517");
    public Guid ApplicationSubmittedToProviderImmediate => new("8b65443f-06b8-4cc9-a83d-5efb847db222");
    public Guid ApplicationSubmittedToProviderWeekly => new("ec70ddac-54c6-4585-adc0-d19bb25b23d9");
    public Guid EmployerVacancyApprovedByDfe => new("d8855c4f-9ce1-4870-93ff-53e609f59a51");
    public Guid EmployerVacancyRejectedByDfe => new("27acd0e9-96fe-47ec-ae33-785e00a453f8");
    public Guid ProviderAttachedToVacancy => new("ce5df943-09ad-4f6c-b10a-42584f88046d");
    public Guid ProviderVacancyApprovedByDfe => new("ee2d7ab3-7ac1-47f8-bc32-86290bda55c9");
    public Guid ProviderVacancyApprovedByEmployer => new("c35e76e7-303b-4b18-bb06-ad98cf68158d");
    public Guid ProviderVacancyRejectedByDfe => new("872e847b-77f5-44a7-b12e-4a19df969ec1");
    public Guid ProviderVacancyRejectedByEmployer => new("8df54598-fea3-45c2-83f3-cca010a6443c");
    public Guid ProviderVacancySentForEmployerReview => new("2b69c0b2-bcc0-4988-82b6-868874e5617b");
    public Guid SharedApplicationReviewedByEmployer => new("2f1b70d4-c722-4815-85a0-80a080eac642");
}

public class DevelopmentEmailTemplateIds : IEmailTemplateIds
{
    public Guid ApplicationSharedWithEmployer => new("f6fc57e6-7318-473d-8cb5-ca653035391a");
    public Guid ApplicationSubmittedToEmployerDaily => new("b793a50f-49f0-4b3f-a4c3-46a8f857e48c");
    public Guid ApplicationSubmittedToEmployerImmediate => new("8aedd294-fd12-4b77-b4b8-2066744e1fdc");
    public Guid ApplicationSubmittedToEmployerWeekly => new("520a434a-2203-49f6-a15a-9e9d1c58c18f");
    public Guid ApplicationSubmittedToProviderDaily => new("f4975bd2-ec66-4f84-a7a6-9693a4f13da3");
    public Guid ApplicationSubmittedToProviderImmediate => new("d9b4b7f3-59ce-46d2-b477-f283f5ab3084");
    public Guid ApplicationSubmittedToProviderWeekly => new("95cc2775-b6f2-4824-a4d9-c394fe0e7aff");
    public Guid EmployerVacancyApprovedByDfe => new("9a45ff1d-769d-4be2-96fb-dcf605e0108f");
    public Guid EmployerVacancyRejectedByDfe => new("5869140a-2a76-4a7c-b4b9-083d2afc5aa5");
    public Guid ProviderAttachedToVacancy => new("b00a94c3-4b6e-48df-b28b-a768600fe7a5");
    public Guid ProviderVacancyApprovedByDfe => new("48c9ab9e-5b13-4843-b4d5-ee1caa46cc64");
    public Guid ProviderVacancyApprovedByEmployer => new("c445095e-e659-499b-b2ab-81e321a9b591");
    public Guid ProviderVacancyRejectedByDfe => new("048d93c9-4371-45a3-96c4-3f93241a5908");
    public Guid ProviderVacancyRejectedByEmployer => new("6e663255-c59d-4964-bcbe-b5881a14c530");
    public Guid ProviderVacancySentForEmployerReview => new("83f6cede-31c3-4dc9-b2ec-922856ba9bdc");
    public Guid SharedApplicationReviewedByEmployer => new("feb4191d-a373-4040-9bc6-93c09d8039b5");
}