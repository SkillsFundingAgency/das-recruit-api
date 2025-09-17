using System.Text.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Configuration;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Data.User;
using SFA.DAS.Recruit.Api.Data.Vacancy;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.ApplicationReview;

public class NewApplicationEmailStrategy(
    ILogger<NewApplicationEmailStrategy> logger,
    IVacancyRepository vacancyRepository,
    IUserRepository userRepository,
    INotificationsRepository notificationsRepository,
    IEncodingService encodingService,
    EmailTemplateHelper emailTemplateHelper) : IApplicationReviewEmailStrategy
{
    // TODO: check these urls
    private const string ApplicationReviewEmployerUrl = "{0}/accounts/{1}/vacancies/{2}/manage";
    private const string ApplicationReviewProviderUrl = "{0}/{1}/vacancies/{2}/manage";
    
    public async Task<List<NotificationEmail>> ExecuteAsync(ApplicationReviewEntity applicationReview, CancellationToken cancellationToken)
    {
        var vacancy = await vacancyRepository.GetOneByVacancyReferenceAsync(applicationReview.VacancyReference, cancellationToken);
        if (vacancy == null)
        {
            logger.LogError("Whilst processing application review '{ApplicationReviewId}' the associated vacancy could not be found", applicationReview.Id);
            throw new DataIntegrityException();
        }

        var users = vacancy.OwnerType switch {
            OwnerType.Employer => await userRepository.FindUsersByEmployerAccountIdAsync(applicationReview.AccountId, cancellationToken),
            OwnerType.Provider => await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken),
            // TODO: do we need to cover these?
            // OwnerType.External => expr,
            // OwnerType.Unknown => expr,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        string? hashedEmployerAccountId = encodingService.Encode(applicationReview.AccountId, EncodingType.AccountId);
        users.ForEach(NotificationPreferenceDefaults.Update);
        var usersRequiringEmail = users.GetUsersForNotificationType(
            NotificationTypes.ApplicationSubmitted,
            vacancy.ReviewRequestedByUserId ?? vacancy.SubmittedByUserId);

        var usersGroupedByFrequency = usersRequiringEmail.GroupBy(x =>
        {
            if (x.NotificationPreferences.TryGetForEvent(NotificationTypes.ApplicationSubmitted, out NotificationPreference? pref))
            {
                return pref.Frequency switch {
                    NotificationFrequency.NotSet => NotificationFrequency.Daily, // default this value
                    _ => pref.Frequency
                };
            }

            return NotificationFrequency.Never;
        }).ToList();
        
        var now = DateTime.Now;
        var nextDailySendDate = now.GetNextDailySendDate();
        var nextWeeklySendDate = now.GetNextWeeklySendDate();
        var templateId = emailTemplateHelper.GetTemplateId(EmailTemplates.ApplicationSubmitted);

        List<NotificationEmail> immediateEmails = [];
        List<RecruitNotificationEntity> delayedEmails = [];
        foreach (var group in usersGroupedByFrequency)
        {
            switch (group.Key)
            {
                case NotificationFrequency.Immediately:
                    immediateEmails.AddRange(group.Select(x => GenerateNotificationEmail(x, applicationReview, vacancy, hashedEmployerAccountId, templateId)));
                    break;
                case NotificationFrequency.Daily:
                    delayedEmails.AddRange(group.Select(x => GenerateDelayedNotification(nextDailySendDate, x, applicationReview, vacancy, hashedEmployerAccountId, templateId)));
                    break;
                case NotificationFrequency.Weekly:
                    delayedEmails.AddRange(group.Select(x => GenerateDelayedNotification(nextWeeklySendDate, x, applicationReview, vacancy, hashedEmployerAccountId, templateId)));
                    break;
            }
        }

        if (delayedEmails.Count > 0)
        {
            // TODO: how do we combine these at a later point?
            // The current endpoint in the controller doesn't understand how to combine dynamic data tokens based on the template id
            await notificationsRepository.InsertManyAsync(delayedEmails, cancellationToken);
        }
        
        return immediateEmails;
    }

    private RecruitNotificationEntity GenerateDelayedNotification(
        DateTime sendWhen,
        UserEntity user,
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy,
        string hashedEmployerAccountId,
        Guid templateId)
    {
        var notification = GenerateNotificationEmail(user, applicationReview, vacancy, hashedEmployerAccountId, templateId);
        return new RecruitNotificationEntity {
            UserId = user.Id,
            SendWhen = sendWhen,
            EmailTemplateId = notification.TemplateId,
            StaticData = JsonSerializer.Serialize(notification.Tokens, JsonConfig.Options), 
            DynamicData = JsonSerializer.Serialize(new Dictionary<string, string>(), JsonConfig.Options),
        };
    }

    private NotificationEmail GenerateNotificationEmail(
        UserEntity user,
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy,
        string hashedEmployerAccountId,
        Guid templateId)
    {
        string ukprn = vacancy.Ukprn!.Value.ToString();
        string manageVacancyUrl = user.UserType switch {
            UserType.Employer => string.Format(ApplicationReviewEmployerUrl, emailTemplateHelper.RecruitEmployerBaseUrl, hashedEmployerAccountId, vacancy.Id),
            UserType.Provider => string.Format(ApplicationReviewProviderUrl, emailTemplateHelper.RecruitProviderBaseUrl, ukprn, vacancy.Id),
        };
        
        string manageNotificationsUrl = user.UserType switch {
            UserType.Employer => emailTemplateHelper.EmployerManageNotificationsUrl(hashedEmployerAccountId),
            UserType.Provider => emailTemplateHelper.ProviderManageNotificationsUrl(ukprn),
        };
        
        return new NotificationEmail
        {
            TemplateId = templateId,
            RecipientAddress = user.Email,
            Tokens = new Dictionary<string, string> {
                ["firstName"] = user.Name,
                ["advertTitle"] = vacancy.Title!,
                ["employerName"] = vacancy.EmployerName!, 
                ["vacancyReference"] = new VacancyReference(applicationReview.VacancyReference).ToShortString(),
                ["manageAdvertURL"] = manageVacancyUrl,
                ["notificationSettingsURL"] = manageNotificationsUrl,
                ["location"] = GetLocationText(vacancy),
            },
        };
    }

    private static string GetLocationText(VacancyEntity vacancy)
    {
        if (vacancy.EmployerLocationOption == AvailableWhere.AcrossEngland)
        {
            return "Recruiting nationally";
        }
        
        var addresses = ApiUtils.DeserializeOrNull<List<Address>>(vacancy.EmployerLocations);
        return addresses is null
            ? string.Empty
            : addresses.GetCityNames();
    }
}