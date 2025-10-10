using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;

public class ApplicationSharedWithEmployerNotificationFactory(
    ILogger<ApplicationSharedWithEmployerNotificationFactory> logger,
    IVacancyRepository vacancyRepository,
    IUserRepository userRepository,
    IEncodingService encodingService,
    IEmailTemplateHelper emailTemplateHelper) : IApplicationReviewNotificationFactory
{
    private const string ApplicationReviewSharedEmployerUrl = "{0}/accounts/{1}/vacancies/{2}/applications/{3}/?vacancySharedByProvider=True";
    
    public async Task<RecruitNotificationsResult> CreateAsync(ApplicationReviewEntity applicationReview, CancellationToken cancellationToken)
    {
        var vacancy = await vacancyRepository.GetOneByVacancyReferenceAsync(applicationReview.VacancyReference, cancellationToken);
        if (vacancy == null)
        {
            logger.LogError("Whilst processing application review '{ApplicationReviewId}' the associated vacancy could not be found", applicationReview.Id);
            throw new DataIntegrityException();
        }

        if (vacancy is not { OwnerType: OwnerType.Provider, Ukprn: not null })
        {
            return new RecruitNotificationsResult();
        }
        
        var usersRequiringEmail = await userRepository.FindUsersByEmployerAccountIdAsync(applicationReview.AccountId, cancellationToken);

        string? employerAccountId = encodingService.Encode(applicationReview.AccountId, EncodingType.AccountId);
        string employerReviewUrl = string.Format(ApplicationReviewSharedEmployerUrl,
            emailTemplateHelper.RecruitEmployerBaseUrl,
            employerAccountId,
            vacancy.Id,
            applicationReview.ApplicationId
        );
        
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.ApplicationSharedWithEmployer,
            UserId = x.Id,
            SendWhen = DateTime.Now,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["firstName"] = x.Name,
                ["trainingProvider"] = vacancy.TrainingProvider_Name!,
                ["advertTitle"] = vacancy.Title!,
                ["vacancyReference"] = new VacancyReference(applicationReview.VacancyReference).ToShortString(),
                ["applicationUrl"] = employerReviewUrl
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });

        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
}