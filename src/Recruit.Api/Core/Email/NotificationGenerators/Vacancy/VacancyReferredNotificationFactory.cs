using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public class VacancyReferredNotificationFactory(
    IUserRepository userRepository,
    IEncodingService encodingService,
    IEmailTemplateHelper emailTemplateHelper): IVacancyNotificationFactory
{
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        if (vacancy is not { Status: VacancyStatus.Approved })
        {
            return new RecruitNotificationsResult();
        }

        return vacancy switch
        {
            { OwnerType: OwnerType.Provider } => await HandleProviderVacancyReferredByQaAsync(vacancy, cancellationToken),
            { OwnerType: OwnerType.Employer } => await HandleEmployerVacancyReferredByQaAsync(vacancy, cancellationToken),
            _ => new RecruitNotificationsResult()
        };
    }

    private async Task<RecruitNotificationsResult> HandleEmployerVacancyReferredByQaAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        var usersWhoMightRequireEmail = await userRepository.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, cancellationToken);
        
        // update with the default notification preferences
        usersWhoMightRequireEmail.ForEach(NotificationPreferenceDefaults.Update);
        
        var usersRequiringEmail = usersWhoMightRequireEmail.GetUsersForNotificationType(
            NotificationTypes.VacancyApprovedOrRejected,
            vacancy.SubmittedByUserId);
        
        var hashedEmployerAccountId = encodingService.Encode(vacancy.AccountId.Value, EncodingType.AccountId);
        var now = DateTime.UtcNow.Date;
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.EmployerVacancyRejectedByDfe,
            UserId = x.Id,
            SendWhen = now,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["advertTitle"] = vacancy.Title!,
                ["firstName"] = x.Name,
                ["employerName"] = vacancy.EmployerName!,
                ["rejectedAdvertURL"] = emailTemplateHelper.EmployerReviewVacancyUrl(hashedEmployerAccountId, vacancy.Id),
                ["notificationSettingsURL"] = emailTemplateHelper.EmployerManageNotificationsUrl(hashedEmployerAccountId),
                ["VACcode"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });
        
        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }

    private async Task<RecruitNotificationsResult> HandleProviderVacancyReferredByQaAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        var usersWhoMightRequireEmail = await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken);
        
        // update with the default notification preferences
        usersWhoMightRequireEmail.ForEach(NotificationPreferenceDefaults.Update);
        
        var usersRequiringEmail = usersWhoMightRequireEmail.GetUsersForNotificationType(
            NotificationTypes.VacancyApprovedOrRejected,
            vacancy.ReviewRequestedByUserId ?? vacancy.SubmittedByUserId);
        
        var now = DateTime.UtcNow.Date;
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.ProviderVacancyRejectedByDfe,
            UserId = x.Id,
            SendWhen = now,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["advertTitle"] = vacancy.Title!,
                ["firstName"] = x.Name,
                ["employerName"] = vacancy.EmployerName!,
                ["rejectedAdvertURL"] = emailTemplateHelper.ProviderReviewVacancyUrl(vacancy.Ukprn!.Value, vacancy.Id),
                ["notificationSettingsURL"] = emailTemplateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn.Value.ToString()),
                ["VACcode"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });

        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
}