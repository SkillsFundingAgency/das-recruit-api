using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public class VacancyApprovedNotificationFactory(
    IUserRepository userRepository,
    IEncodingService encodingService,
    IEmailTemplateHelper emailTemplateHelper): IVacancyNotificationFactory
{
    private readonly DateTime _sendWhen = DateTime.UtcNow.Date;
    
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        if (vacancy is not { Status: VacancyStatus.Approved })
        {
            return new RecruitNotificationsResult();
        }
        
        return await HandleVacancyApprovedByQaAsync(vacancy, cancellationToken); 
    }

    private async Task<RecruitNotificationsResult> HandleVacancyApprovedByQaAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        var result = new RecruitNotificationsResult();
        var pasUsersForUkprn = await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken);
        var liveUrl = emailTemplateHelper.FaaVacancyUrl(vacancy.VacancyReference);

        if (vacancy is { OwnerType: OwnerType.Provider } && HandleProviderApprovedNotifications(vacancy, pasUsersForUkprn, liveUrl, out var pasNotifications))
        {
            result.Immediate.AddRange(pasNotifications);
        }

        if (vacancy is not { OwnerType: OwnerType.Employer })
        {
            return result;
        }
        
        var easUsersForAccount = await userRepository.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, cancellationToken);
        if (HandleEmployerApprovedNotifications(vacancy, easUsersForAccount, liveUrl, out var notifications))
        {
            result.Immediate.AddRange(notifications);
        }
            
        // notify providers (attached)
        if (HandleProviderAttachedNotifications(vacancy, easUsersForAccount, pasUsersForUkprn, out var attachedNotifications))
        {
            result.Immediate.AddRange(attachedNotifications);
        }

        return result;
    }

    private bool HandleProviderAttachedNotifications(VacancyEntity vacancy, List<UserEntity> easUsersForAccount, List<UserEntity> pasUsersForUkprn, [NotNullWhen(true)]out List<RecruitNotificationEntity>? notifications)
    {
        var providerUsersRequiringAttachedEmail = pasUsersForUkprn.GetUsersForNotificationType(NotificationTypes.ProviderAttachedToVacancy);
        if (providerUsersRequiringAttachedEmail is { Count: 0 })
        {
            notifications = null;
            return false;
        }
        
        var apprenticeCount = vacancy.NumberOfPositions > 1 ? $"{vacancy.NumberOfPositions} apprentices" : "1 apprentice";
        var submittingUser = easUsersForAccount.Find(x => x.Id == vacancy.SubmittedByUserId);
        notifications = providerUsersRequiringAttachedEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.ProviderAttachedToVacancy,
            UserId = x.Id,
            SendWhen = _sendWhen,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["firstName"] = x.Name,
                ["advertTitle"] = vacancy.Title!,
                ["VACnumber"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
                ["employer"] = vacancy.EmployerName!,
                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
                ["applicationUrl"] = emailTemplateHelper.FaaVacancyUrl(vacancy.VacancyReference),
                ["notificationSettingsURL"] = emailTemplateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()),
                ["positions"] = apprenticeCount,
                ["startDate"] = vacancy.StartDate.ToDayMonthYearString(),
                ["duration"] = vacancy.GetWageDuration(),
                ["submitterEmail"] = submittingUser?.Email ?? "Contact details not found",
                //["courseTitle"] = "",
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        }).ToList();

        return true;
    }

    private bool HandleEmployerApprovedNotifications(VacancyEntity vacancy, List<UserEntity> easUsersForAccount, string liveUrl, [NotNullWhen(true)]out List<RecruitNotificationEntity>? notifications)
    {
        var easUsersToEmail = easUsersForAccount.GetUsersForNotificationType(NotificationTypes.VacancyApprovedOrRejected, vacancy.SubmittedByUserId);
        if (easUsersToEmail is { Count: 0 })
        {
            notifications = null;
            return false;
        }
        
        var hashedEmployerAccountId = encodingService.Encode(vacancy.AccountId!.Value, EncodingType.AccountId);
        notifications = easUsersToEmail.Select(x => GetApprovedNotificationEntity(
            emailTemplateHelper.TemplateIds.EmployerVacancyApprovedByDfe,
            vacancy,
            x,
            liveUrl,
            emailTemplateHelper.EmployerManageNotificationsUrl(hashedEmployerAccountId))).ToList();
        
        return true;
    }

    private bool HandleProviderApprovedNotifications(VacancyEntity vacancy, List<UserEntity> pasUsersForUkprn, string liveUrl, [NotNullWhen(true)]out List<RecruitNotificationEntity>? notifications)
    {
        var pasUsersToEmail = pasUsersForUkprn.GetUsersForNotificationType(NotificationTypes.VacancyApprovedOrRejected, vacancy.ReviewRequestedByUserId ?? vacancy.SubmittedByUserId);
        if (pasUsersToEmail is { Count: 0 })
        {
            notifications = null;
            return false;
        }
        
        notifications = pasUsersToEmail.Select(x => GetApprovedNotificationEntity(
            emailTemplateHelper.TemplateIds.ProviderVacancyApprovedByDfe,
            vacancy,
            x,
            liveUrl,
            emailTemplateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()))).ToList();
        
        return true;
    }

    private RecruitNotificationEntity GetApprovedNotificationEntity(
        Guid templateId,
        VacancyEntity vacancy,
        UserEntity user,
        string faaUrl,
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
                ["FindAnApprenticeshipAdvertURL"] = faaUrl,
                ["notificationSettingsURL"] = manageNotificationsUrl,
                ["VACcode"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        };
    }
}