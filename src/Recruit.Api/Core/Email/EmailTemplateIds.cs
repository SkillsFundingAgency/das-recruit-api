namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IEmailTemplateIds
{
    Guid ApplicationSubmittedToEmployerImmediate { get; }
    Guid ApplicationSubmittedToEmployerDaily { get; }
    Guid ApplicationSubmittedToEmployerWeekly { get; }
    Guid ApplicationSubmittedToProviderImmediate { get; }
    Guid ApplicationSubmittedToProviderDaily { get; }
    Guid ApplicationSubmittedToProviderWeekly { get; }
    Guid ApplicationSharedWithEmployer { get; }
    Guid SharedApplicationReviewedByEmployer { get; }
    Guid ProviderVacancySentForEmployerReview { get; }
    Guid ProviderVacancyApprovedByEmployer { get; }
    Guid ProviderVacancyRejectedByEmployer { get; }
}

public class ProductionEmailTemplateIds : IEmailTemplateIds
{
    public Guid ApplicationSubmittedToEmployerImmediate { get; } = new("e07a6992-4d17-4167-b526-2ead6fe9ad4d");
    public Guid ApplicationSubmittedToEmployerDaily { get; } = new("1c8c9e72-86c1-4fd1-8020-f4fe354a6e79");
    public Guid ApplicationSubmittedToEmployerWeekly { get; } = new ("68d467ac-339c-42b4-b862-ca06e1cc66e8");
    public Guid ApplicationSubmittedToProviderImmediate { get; } = new("8b65443f-06b8-4cc9-a83d-5efb847db222");
    public Guid ApplicationSubmittedToProviderDaily { get; } = new("8e38bbdc-9632-465b-95b7-01523570e517");
    public Guid ApplicationSubmittedToProviderWeekly { get; } = new("ec70ddac-54c6-4585-adc0-d19bb25b23d9");
    public Guid ApplicationSharedWithEmployer { get; } = new("53058846-e369-4396-87b2-015c9d16360a");
    public Guid SharedApplicationReviewedByEmployer { get; } = new("2f1b70d4-c722-4815-85a0-80a080eac642");
    public Guid ProviderVacancySentForEmployerReview { get; } = new("2b69c0b2-bcc0-4988-82b6-868874e5617b");
    public Guid ProviderVacancyApprovedByEmployer { get; } = new("c35e76e7-303b-4b18-bb06-ad98cf68158d");
    public Guid ProviderVacancyRejectedByEmployer { get; } = new("8df54598-fea3-45c2-83f3-cca010a6443c");
}

public class DevelopmentEmailTemplateIds : IEmailTemplateIds
{
    public Guid ApplicationSubmittedToEmployerImmediate { get; } = new("8aedd294-fd12-4b77-b4b8-2066744e1fdc");
    public Guid ApplicationSubmittedToEmployerDaily { get; } = new("b793a50f-49f0-4b3f-a4c3-46a8f857e48c");
    public Guid ApplicationSubmittedToEmployerWeekly { get; } = new("520a434a-2203-49f6-a15a-9e9d1c58c18f");
    public Guid ApplicationSubmittedToProviderImmediate { get; } = new("d9b4b7f3-59ce-46d2-b477-f283f5ab3084");
    public Guid ApplicationSubmittedToProviderDaily { get; } = new("f4975bd2-ec66-4f84-a7a6-9693a4f13da3");
    public Guid ApplicationSubmittedToProviderWeekly { get; } = new("95cc2775-b6f2-4824-a4d9-c394fe0e7aff");
    public Guid ApplicationSharedWithEmployer { get; } = new("f6fc57e6-7318-473d-8cb5-ca653035391a");
    public Guid SharedApplicationReviewedByEmployer { get; } = new("feb4191d-a373-4040-9bc6-93c09d8039b5");
    public Guid ProviderVacancySentForEmployerReview { get; } = new("83f6cede-31c3-4dc9-b2ec-922856ba9bdc");
    public Guid ProviderVacancyApprovedByEmployer { get; } = new("c445095e-e659-499b-b2ab-81e321a9b591");
    public Guid ProviderVacancyRejectedByEmployer { get; } = new("6e663255-c59d-4964-bcbe-b5881a14c530");
}