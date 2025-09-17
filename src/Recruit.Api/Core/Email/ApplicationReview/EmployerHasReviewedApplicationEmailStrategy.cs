using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.ApplicationReview;

internal class EmployerHasReviewedApplicationEmailStrategy(
    ILogger<EmployerHasReviewedApplicationEmailStrategy> logger,
    IVacancyRepository vacancyRepository,
    IUserRepository userRepository,
    EmailTemplateHelper emailTemplateHelper) : IApplicationReviewEmailStrategy
{
    private const string ProviderManageVacancyUrl = "{0}/{1}/vacancies/{2}/manage";
    
    public async Task<List<NotificationEmail>> ExecuteAsync(ApplicationReviewEntity applicationReview, CancellationToken cancellationToken)
    {
        var vacancy = await vacancyRepository.GetOneByVacancyReferenceAsync(applicationReview.VacancyReference, cancellationToken);
        if (vacancy == null)
        {
            logger.LogError("Whilst processing application review '{ApplicationReviewId}' the associated vacancy could not be found", applicationReview.Id);
            throw new DataIntegrityException();
        }
        
        var providerUsers = await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken);
        providerUsers.ForEach(NotificationPreferenceDefaults.Update);
        var usersRequiringEmail = providerUsers.GetUsersForNotificationType(
            NotificationTypes.SharedApplicationReviewedByEmployer,
            vacancy.ReviewRequestedByUserId ?? vacancy.SubmittedByUserId);
        
        string ukprn = vacancy.Ukprn!.Value.ToString();
        return usersRequiringEmail.Select(x => new NotificationEmail {
            TemplateId = emailTemplateHelper.GetTemplateId(EmailTemplates.EmployerHasReviewedSharedApplication),
            RecipientAddress = x.Email,
            Tokens = new Dictionary<string, string> {
                ["firstName"] = x.Name,
                ["employer"] = vacancy.EmployerName!,
                ["advertTitle"] = vacancy.Title!,
                ["vacancyReference"] = new VacancyReference(applicationReview.VacancyReference).ToShortString(),
                ["manageVacancyURL"] = string.Format(ProviderManageVacancyUrl, emailTemplateHelper.RecruitProviderBaseUrl, ukprn, vacancy.Id),
                ["notificationSettingsURL"] = emailTemplateHelper.ProviderManageNotificationsUrl(ukprn)
            },
        }).ToList();
    }
}