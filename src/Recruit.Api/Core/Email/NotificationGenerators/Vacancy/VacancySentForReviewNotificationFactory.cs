using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public class VacancySentForReviewNotificationFactory(
    IUserRepository userRepository,
    IEncodingService encodingService,
    IEmailTemplateHelper emailTemplateHelper): IVacancyNotificationFactory
{
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        if (vacancy is not { Status: VacancyStatus.Review, OwnerType: OwnerType.Provider, ReviewRequestedByUserId: not null })
        {
            return new RecruitNotificationsResult();
        }
        
        var usersRequiringEmail = await userRepository.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, cancellationToken);
        var hashedEmployerAccountId = encodingService.Encode(vacancy.AccountId.Value, EncodingType.AccountId);
        var now = DateTime.UtcNow.Date;
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.ProviderVacancySentForEmployerReview,
            UserId = x.Id,
            SendWhen = now,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["firstName"] = x.Name,
                ["trainingProviderName"] = vacancy.TrainingProvider_Name!,
                ["advertTitle"] = vacancy.Title!,
                ["vacancyReference"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
                ["employerName"] = vacancy.EmployerName!,
                ["location"] = vacancy.GetLocationText(JsonConfig.Options),
                ["reviewAdvertURL"] = emailTemplateHelper.EmployerReviewVacancyUrl(hashedEmployerAccountId, vacancy.Id),
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });

        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
}