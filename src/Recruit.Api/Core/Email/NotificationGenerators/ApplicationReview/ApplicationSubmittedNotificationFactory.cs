using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Configuration;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;

public class ApplicationSubmittedNotificationFactory(
    ILogger<ApplicationSubmittedNotificationFactory> logger,
    IVacancyRepository vacancyRepository,
    IUserRepository userRepository,
    IEncodingService encodingService,
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

        // fetch our user set depending on the type of vacancy
        var users = vacancy.OwnerType switch {
            OwnerType.Employer => await userRepository.FindUsersByEmployerAccountIdAsync(applicationReview.AccountId, cancellationToken),
            OwnerType.Provider => await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken),
            _ => throw new EntityStateNotSupportedException($"The vacancy owner type '{vacancy.OwnerType}' is not supported")
        };
        
        // update with the default notification preferences
        users.ForEach(NotificationPreferenceDefaults.Update);
        
        // filter down to those who actually want to receive the notification
        var usersRequiringEmail = users.GetUsersForNotificationType(
            NotificationTypes.ApplicationSubmitted,
            vacancy.ReviewRequestedByUserId ?? vacancy.SubmittedByUserId);

        // group the users by the frequency of their notification preference
        var usersGroupedByFrequency = usersRequiringEmail.GroupBy(x =>
        {
            if (x.NotificationPreferences.TryGetForEvent(NotificationTypes.ApplicationSubmitted, out NotificationPreference? pref))
            {
                return pref.Frequency switch {
                    NotificationFrequency.NotSet => NotificationFrequency.Daily, // default this value to daily
                    _ => pref.Frequency
                };
            }

            return NotificationFrequency.Never;
        });
        
        string? hashedEmployerAccountId = encodingService.Encode(applicationReview.AccountId, EncodingType.AccountId);
        string ukprn = vacancy.Ukprn!.Value.ToString();
        var now = DateTime.Now;
        
        // process each frequency
        var results = new RecruitNotificationsResult();
        foreach (var group in usersGroupedByFrequency)
        {
            switch (group.Key)
            {
                case NotificationFrequency.Immediately:
                    {
                        var recruitNotifications = group.Select(x => new RecruitNotificationEntity {
                            EmailTemplateId = x.UserType == UserType.Employer 
                                ? emailTemplateHelper.TemplateIds.ApplicationSubmittedToEmployerImmediate
                                : emailTemplateHelper.TemplateIds.ApplicationSubmittedToProviderImmediate,
                            UserId = x.Id,
                            User = x,
                            SendWhen = DateTime.Now,
                            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                                ["firstName"] = x.Name,
                                ["advertTitle"] = vacancy.Title!,
                                ["employerName"] = vacancy.EmployerName!,
                                ["vacancyReference"] = new VacancyReference(applicationReview.VacancyReference).ToShortString(),
                                ["manageVacancyURL"] = ManageVacancyUrl(x),
                                ["notificationSettingsURL"] = ManageNotificationsUrl(x),
                                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
                            })!,
                            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
                        });
                        results.Immediate.AddRange(recruitNotifications);
                        break;
                    }
                case NotificationFrequency.Daily:
                    {
                        var recruitNotifications = group.Select(x => new RecruitNotificationEntity {
                            EmailTemplateId = x.UserType == UserType.Employer 
                                ? emailTemplateHelper.TemplateIds.ApplicationSubmittedToEmployerDaily
                                : emailTemplateHelper.TemplateIds.ApplicationSubmittedToProviderDaily,
                            UserId = x.Id,
                            User = x,
                            SendWhen = now.GetNextDailySendDate(),
                            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                                ["firstName"] = x.Name,
                                ["notificationSettingsURL"] = ManageNotificationsUrl(x),
                            })!,
                            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                                ["advertTitle"] = vacancy.Title!,
                                ["employerName"] = vacancy.EmployerName!,
                                ["vacancyReference"] = new VacancyReference(applicationReview.VacancyReference).ToShortString(),
                                ["manageVacancyURL"] = ManageVacancyUrl(x),
                                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
                            })!,
                        });
                        results.Delayed.AddRange(recruitNotifications);
                        break;
                    }
                case NotificationFrequency.Weekly:
                    {
                        var recruitNotifications = group.Select(x => new RecruitNotificationEntity {
                            EmailTemplateId = x.UserType == UserType.Employer 
                                ? emailTemplateHelper.TemplateIds.ApplicationSubmittedToEmployerWeekly
                                : emailTemplateHelper.TemplateIds.ApplicationSubmittedToProviderWeekly,
                            UserId = x.Id,
                            User = x,
                            SendWhen = now.GetNextWeeklySendDate(),
                            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                                ["firstName"] = x.Name,
                                ["notificationSettingsURL"] = ManageNotificationsUrl(x),
                            })!,
                            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                                ["advertTitle"] = vacancy.Title!,
                                ["employerName"] = vacancy.EmployerName!,
                                ["vacancyReference"] = new VacancyReference(applicationReview.VacancyReference).ToShortString(),
                                ["manageVacancyURL"] = ManageVacancyUrl(x),
                                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
                            })!,
                        });
                        results.Delayed.AddRange(recruitNotifications);
                        break;
                    }
            }
        }

        return results;

        // inline utility functions
        string ManageNotificationsUrl(UserEntity user) => user.UserType switch {
            UserType.Employer => emailTemplateHelper.EmployerManageNotificationsUrl(hashedEmployerAccountId),
            UserType.Provider => emailTemplateHelper.ProviderManageNotificationsUrl(ukprn),
            _ => string.Empty
        };

        string ManageVacancyUrl(UserEntity user) => user.UserType switch {
            UserType.Employer => emailTemplateHelper.EmployerManageVacancyUrl(hashedEmployerAccountId, vacancy.Id),
            UserType.Provider => emailTemplateHelper.ProviderManageVacancyUrl(ukprn, vacancy.Id),
            _ => string.Empty
        };
    }
}