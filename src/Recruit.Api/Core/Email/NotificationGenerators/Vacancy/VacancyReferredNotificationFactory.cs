using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Data.Repositories;
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
    private readonly DateTime _sendWhen = DateTime.UtcNow.Date;
    
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, Dictionary<string, string> data, CancellationToken cancellationToken)
    {
        if (vacancy is not { Status: VacancyStatus.Referred })
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
        var easUsersForAccount = await userRepository.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, cancellationToken);
        var usersRequiringEmail = easUsersForAccount.GetUsersForNotificationType(
            NotificationTypes.VacancyApprovedOrRejected,
            vacancy.SubmittedByUserId);
        
        if (usersRequiringEmail is { Count: 0 })
        {
            return new RecruitNotificationsResult();
        }
        
        var hashedEmployerAccountId = encodingService.Encode(vacancy.AccountId.Value, EncodingType.AccountId);
        var recruitNotifications = usersRequiringEmail.Select(x => GetReferredNotificationEntity(
            emailTemplateHelper.TemplateIds.EmployerVacancyRejectedByDfe,
            vacancy,
            x,
            emailTemplateHelper.EmployerReviewVacancyUrl(hashedEmployerAccountId, vacancy.Id),
            emailTemplateHelper.EmployerManageNotificationsUrl(hashedEmployerAccountId)));
        
        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }

    private async Task<RecruitNotificationsResult> HandleProviderVacancyReferredByQaAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        var pasUsersForUkprn = await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken);
        var usersRequiringEmail = pasUsersForUkprn.GetUsersForNotificationType(
            NotificationTypes.VacancyApprovedOrRejected,
            vacancy.ReviewRequestedByUserId ?? vacancy.SubmittedByUserId);

        if (usersRequiringEmail is { Count: 0 })
        {
            return new RecruitNotificationsResult();
        }
        
        var recruitNotifications = usersRequiringEmail.Select(x => GetReferredNotificationEntity(
            emailTemplateHelper.TemplateIds.ProviderVacancyRejectedByDfe,
            vacancy,
            x,
            emailTemplateHelper.ProviderReviewVacancyUrl(vacancy.Ukprn!.Value, vacancy.Id),
            emailTemplateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn.Value.ToString())));

        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
    
    private RecruitNotificationEntity GetReferredNotificationEntity(
        Guid templateId,
        VacancyEntity vacancy,
        UserEntity user,
        string manageVacancyUrl,
        string manageNotificationsUrl)
        
    {
        return new RecruitNotificationEntity {
            EmailTemplateId = templateId,
            UserId = user.Id,
            SendWhen = _sendWhen,
            User = user,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["advertTitle"] = vacancy.Title!,
                ["firstName"] = user.Name,
                ["employerName"] = vacancy.EmployerName!,
                ["rejectedAdvertURL"] = manageVacancyUrl,
                ["notificationSettingsURL"] = manageNotificationsUrl,
                ["VACcode"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        };
    }
}