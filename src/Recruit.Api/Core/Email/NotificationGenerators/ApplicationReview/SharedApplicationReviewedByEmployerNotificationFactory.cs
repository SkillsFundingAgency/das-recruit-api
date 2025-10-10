using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;

public class SharedApplicationReviewedByEmployerNotificationFactory(
    ILogger<SharedApplicationReviewedByEmployerNotificationFactory> logger,
    IVacancyRepository vacancyRepository,
    IUserRepository userRepository,
    IEmailTemplateHelper emailTemplateHelper) : IApplicationReviewNotificationFactory
{
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
        
        var providerUsers = await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken);
        var usersRequiringEmail = providerUsers.GetUsersForNotificationType(
            NotificationTypes.SharedApplicationReviewedByEmployer, vacancy.ReviewRequestedByUserId);
        
        string ukprn = vacancy.Ukprn!.Value.ToString();
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.SharedApplicationReviewedByEmployer,
            UserId = x.Id,
            SendWhen = DateTime.Now,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["firstName"] = x.Name,
                ["employer"] = vacancy.EmployerName!,
                ["advertTitle"] = vacancy.Title!,
                ["vacancyReference"] = new VacancyReference(applicationReview.VacancyReference).ToShortString(),
                ["manageVacancyURL"] = emailTemplateHelper.ProviderManageVacancyUrl(ukprn, vacancy.Id),
                ["notificationSettingsURL"] = emailTemplateHelper.ProviderManageNotificationsUrl(ukprn)
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });

        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
}