using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public class VacancyRejectedNotificationFactory(
    IUserRepository userRepository,
    IEmailTemplateHelper emailTemplateHelper): IVacancyNotificationFactory
{
    private const string ProviderReviewVacancyUrl = "{0}/{1}/vacancies/{2}/check-your-answers";
    
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        if (vacancy is not { Status: VacancyStatus.Rejected })
        {
            return new RecruitNotificationsResult();
        }

        if (vacancy is { OwnerType: OwnerType.Provider, EmployerRejectedReason: not null })
        {
            // this will handle when the employer rejects a vacancy from a provider
            return await HandleVacancyRejectedByEmployerAsync(vacancy, cancellationToken);
        }

        return new RecruitNotificationsResult();
    }

    private async Task<RecruitNotificationsResult> HandleVacancyRejectedByEmployerAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        var usersWhoMightRequireEmail = await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken);
        
        // update with the default notification preferences
        usersWhoMightRequireEmail.ForEach(NotificationPreferenceDefaults.Update);
        
        var usersRequiringEmail = usersWhoMightRequireEmail.GetUsersForNotificationType(
            NotificationTypes.VacancyApprovedOrRejected,
            vacancy.ReviewRequestedByUserId);
        
        var now = DateTime.UtcNow.Date;
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.ProviderVacancyRejectedByEmployer,
            UserId = x.Id,
            SendWhen = now,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["firstName"] = x.Name,
                ["vacancyTitle"] = vacancy.Title!,
                ["vacancyReference"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
                ["employerName"] = vacancy.EmployerName!,
                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
                ["rejectedEmployerVacancyURL"] = string.Format(ProviderReviewVacancyUrl, emailTemplateHelper.RecruitProviderBaseUrl, vacancy.Ukprn, vacancy.Id),
                ["notificationSettingsURL"] = emailTemplateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn.Value.ToString()),
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });

        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
}