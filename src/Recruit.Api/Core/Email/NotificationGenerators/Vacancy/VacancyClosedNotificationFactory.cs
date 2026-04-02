using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public class VacancyClosedNotificationFactory(
    IUserRepository userRepository,
    IEmailTemplateHelper emailTemplateHelper): IVacancyNotificationFactory
{
    public async Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, Dictionary<string, string> data, CancellationToken cancellationToken)
    {
        if (vacancy is not { Status: VacancyStatus.Closed })
        {
            return new RecruitNotificationsResult();
        }

        if (vacancy is { ClosureReason: ClosureReason.WithdrawnByQa })
        {
            return vacancy switch
            {
                { OwnerType: OwnerType.Provider } => await HandleProviderVacancyWithdrawnByQaAsync(vacancy, cancellationToken),
                { OwnerType: OwnerType.Employer } => await HandleEmployerVacancyWithdrawnByQaAsync(vacancy, cancellationToken),
                _ => new RecruitNotificationsResult()
            };
        }
        
        return new RecruitNotificationsResult();
    }

    private async Task<RecruitNotificationsResult> HandleProviderVacancyWithdrawnByQaAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        var pasUsersForUkprn = await userRepository.FindUsersByUkprnAsync(vacancy.Ukprn!.Value, cancellationToken);
        var usersRequiringEmail = pasUsersForUkprn.ActiveInTheLastYear();
        return GenerateNotifications(usersRequiringEmail, vacancy);
    }
    
    private async Task<RecruitNotificationsResult> HandleEmployerVacancyWithdrawnByQaAsync(VacancyEntity vacancy, CancellationToken cancellationToken)
    {
        var easUsersForAccount = await userRepository.FindUsersByEmployerAccountIdAsync(vacancy.AccountId!.Value, cancellationToken);
        var usersRequiringEmail = easUsersForAccount.ActiveInTheLastYear();
        return GenerateNotifications(usersRequiringEmail, vacancy);
    }

    private RecruitNotificationsResult GenerateNotifications(List<UserEntity> usersRequiringEmail, VacancyEntity vacancy)
    {
        if (usersRequiringEmail is { Count: 0 })
        {
            return new RecruitNotificationsResult();
        }
        
        var now = DateTime.UtcNow.Date;
        var recruitNotifications = usersRequiringEmail.Select(x => new RecruitNotificationEntity {
            EmailTemplateId = emailTemplateHelper.TemplateIds.VacancyWithdrawnByQa,
            UserId = x.Id,
            SendWhen = now,
            User = x,
            StaticData = ApiUtils.SerializeOrNull(new Dictionary<string, string> {
                ["user-name"] = x.Name,
                ["vacancy-title"] = vacancy.Title!,
                ["vacancy-reference"] = new VacancyReference(vacancy.VacancyReference).ToShortString(),
            })!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string>())!
        });
        
        var results = new RecruitNotificationsResult();
        results.Immediate.AddRange(recruitNotifications);
        return results;
    }
}