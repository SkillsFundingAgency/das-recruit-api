using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.FeedbackNudgeEmail;

public class VacancyFeedbackNotificationFactory(
    ILogger<VacancyFeedbackNotificationFactory> logger,
    IEncodingService encodingService,
    IUserRepository userRepository,
    IEmailTemplateHelper emailTemplateHelper): IVacancyFeedbackNotificationFactory
{
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, Dictionary<string, string> data, CancellationToken cancellationToken)
    {
        var result = new RecruitNotificationsResult();
        if (vacancy.Status is not VacancyStatus.Closed)
        {
            logger.LogWarning("Could not generate nudge email for vacancy {VacancyId} the vacancy is not in the closed status (actual:{VacancyStatus})", vacancy.Id, vacancy.Status);
            return result;
        }
        
        if (vacancy.OwnerType is not (OwnerType.Employer or OwnerType.Provider))
        {
            logger.LogWarning("Could not generate nudge email for vacancy {VacancyId} the vacancy is not owned by either Provider or Employer (actual:{OwnerType})", vacancy.Id, vacancy.OwnerType);
            return result;
        }

        var user = vacancy.OwnerType switch {
            OwnerType.Employer => await userRepository.FindByUserIdAsync(vacancy.SubmittedByUserId.ToString()!, cancellationToken),
            OwnerType.Provider => await userRepository.FindByUserIdAsync(vacancy.ReviewRequestedByUserId?.ToString() ?? vacancy.SubmittedByUserId.ToString()!, cancellationToken),
        };

        if (user == null)
        {
            logger.LogWarning("Could not generate nudge email for vacancy {VacancyId} as no owner could be identified", vacancy.Id);
            return result;
        }

        var manageVacancyUrl = string.Empty;
        var advertNoun = string.Empty;

        switch (vacancy.OwnerType)
        {
            case OwnerType.Employer:
                var hashedEmployerAccountId = encodingService.Encode(vacancy.AccountId!.Value, EncodingType.AccountId);
                manageVacancyUrl = emailTemplateHelper.EmployerManageVacancyUrl(hashedEmployerAccountId, vacancy.Id);
                advertNoun = "advert";
                break;
            case OwnerType.Provider:
                manageVacancyUrl = emailTemplateHelper.ProviderManageVacancyUrl($"{vacancy.Ukprn!.Value}", vacancy.Id);
                advertNoun = "vacancy";
                break;
        }
        
        var feedbackRequiredCount = data.GetValueOrDefault("feedbackCount") ?? string.Empty;
     
        result.Immediate.Add(new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.VacancyFeedbackRequired,
            UserId = user.Id,
            SendWhen = DateTime.UtcNow.Date,
            User = user,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["firstName"] = user.Name,
                ["feedbackCount"] = feedbackRequiredCount,
                ["advertNoun"] = advertNoun,
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["advertTitle"] = vacancy.Title!,
                ["manageAdvertURL"] = manageVacancyUrl,
            })!
        });

        return result;
    }
}