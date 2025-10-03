using SFA.DAS.Recruit.Api.Configuration;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public class VacancySubmittedNotificationFactory(
    IUserRepository userRepository,
    IEmailTemplateHelper emailTemplateHelper): IVacancyNotificationFactory
{
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        if (vacancy is not { Status: VacancyStatus.Submitted })
        {
            return new RecruitNotificationsResult();
        }

        if (vacancy is { OwnerType: OwnerType.Provider, ReviewRequestedByUserId: not null })
        {
            // this will handle provider vacancies that have been approved by the employer
            return await HandleProviderOwnedRequiringEmployerReview(vacancy, cancellationToken);
        }

        return new RecruitNotificationsResult();
    }

    private async Task<RecruitNotificationsResult> HandleProviderOwnedRequiringEmployerReview(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        var usersWhoMightRequireEmail = await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken);
        
        // update with the default notification preferences
        usersWhoMightRequireEmail.ForEach(NotificationPreferenceDefaults.Update);
        
        var usersRequiringEmail = usersWhoMightRequireEmail.GetUsersForNotificationType(
            NotificationTypes.VacancyApprovedOrRejected,
            vacancy.ReviewRequestedByUserId ?? vacancy.SubmittedByUserId);
        
        var now = DateTime.UtcNow.Date;
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.GetTemplateId(NotificationTypes.VacancyApprovedOrRejected),
            UserId = x.Id,
            SendWhen = now,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["firstName"] = x.Name,
                ["advertTitle"] = vacancy.Title!,
                ["VACcode"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
                ["employerName"] = vacancy.EmployerName!,
                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
                ["notificationSettingsURL"] = emailTemplateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn.Value.ToString()),
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });

        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
}