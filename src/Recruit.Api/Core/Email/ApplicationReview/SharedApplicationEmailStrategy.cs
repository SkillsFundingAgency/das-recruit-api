using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.User;
using SFA.DAS.Recruit.Api.Data.Vacancy;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.ApplicationReview;

internal class SharedApplicationEmailStrategy(
    ILogger<SharedApplicationEmailStrategy> logger,
    IVacancyRepository vacancyRepository, 
    IUserRepository userRepository,
    IEncodingService encodingService,
    EmailTemplateHelper emailTemplateHelper) : IApplicationReviewEmailStrategy
{
    private const string ApplicationReviewSharedEmployerUrl = "{0}/accounts/{1}/vacancies/{2}/applications/{3}/?vacancySharedByProvider=True";
    
    public async Task<List<NotificationEmail>> ExecuteAsync(ApplicationReviewEntity applicationReview, CancellationToken cancellationToken)
    {
        var vacancy = await vacancyRepository.GetOneByVacancyReferenceAsync(applicationReview.VacancyReference, cancellationToken);
        if (vacancy == null)
        {
            logger.LogError("Whilst processing application review '{ApplicationReviewId}' the associated vacancy could not be found", applicationReview.Id);
            throw new DataIntegrityException();
        }
        
        var usersRequiringEmail = await userRepository.FindUsersByEmployerAccountIdAsync(applicationReview.AccountId, cancellationToken);
        usersRequiringEmail.ForEach(NotificationPreferenceDefaults.Update);

        string? employerAccountId = encodingService.Encode(applicationReview.AccountId, EncodingType.AccountId);
        string employerReviewUrl = string.Format(ApplicationReviewSharedEmployerUrl,
            emailTemplateHelper.RecruitEmployerBaseUrl,
            employerAccountId,
            vacancy.Id,
            applicationReview.ApplicationId
        );

        return usersRequiringEmail.Select(x => new NotificationEmail {
            TemplateId = emailTemplateHelper.GetTemplateId(EmailTemplates.ApplicationReviewShared),
            RecipientAddress = x.Email,
            Tokens = new Dictionary<string, string> {
                ["firstName"] = x.Name,
                ["trainingProvider"] = vacancy.TrainingProvider_Name!,
                ["advertTitle"] = vacancy.Title!,
                ["vacancyReference"] = new VacancyReference(applicationReview.VacancyReference).ToShortString(),
                ["applicationUrl"] = employerReviewUrl
            },
        }).ToList();
    }
}