using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Configuration;
using SFA.DAS.Recruit.Api.Data.Repositories;
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
    private const string EmployerReviewVacancyUrl = "{0}/accounts/{1}/vacancies/{2}/check-answers";
    
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        if (vacancy is not { Status: VacancyStatus.Review, OwnerType: OwnerType.Provider, ReviewRequestedByUserId: not null })
        {
            return new RecruitNotificationsResult();
        }
        
        var usersRequiringEmail = await userRepository.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, cancellationToken);
        string? hashedEmployerAccountId = encodingService.Encode(vacancy.AccountId.Value, EncodingType.AccountId);
        string employerReviewUrl = string.Format(EmployerReviewVacancyUrl,
            emailTemplateHelper.RecruitEmployerBaseUrl,
            hashedEmployerAccountId,
            vacancy.Id);

        var now = DateTime.UtcNow.Date;
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.GetTemplateId(NotificationTypes.VacancySentForReview),
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
                ["reviewAdvertURL"] = employerReviewUrl
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });

        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
}