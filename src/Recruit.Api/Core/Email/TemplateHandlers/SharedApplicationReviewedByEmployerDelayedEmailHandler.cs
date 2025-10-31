using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;

public class SharedApplicationReviewedByEmployerDelayedEmailHandler: AbstractEmailHandler
{
    public SharedApplicationReviewedByEmployerDelayedEmailHandler(IEmailTemplateHelper emailTemplateHelper)
    {
        SupportedTemplates.Add(emailTemplateHelper.TemplateIds.SharedApplicationReviewedByEmployer);
    }
    
    public override IEnumerable<NotificationEmail> CreateNotificationEmails(IEnumerable<RecruitNotificationEntity> recruitNotifications)
    {
        List<NotificationEmail> results = [];

        // process each group of records for a recruit user
        var groupedByUser = recruitNotifications.GroupBy(x => x.UserId);
        foreach (var userGroup in groupedByUser)
        {
            results.AddRange(ProcessUser(userGroup));
        }

        return results;
    }

    private static IEnumerable<NotificationEmail> ProcessUser(IGrouping<Guid, RecruitNotificationEntity> userGroup)
    {
        // group the records for a user by the vacancy 
        var vacancyGroups = userGroup
            .Select(x =>
            {
                var staticData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(x.StaticData)!;
                return new {
                    RecruitNotification = x,
                    StaticData = staticData,
                    VacancyReference = new VacancyReference(staticData["vacancyReference"])
                };
            })
            .GroupBy(x => x.VacancyReference)
            .ToList();

        return vacancyGroups.Select(x =>
        {
            var info = x.First();
            return new NotificationEmail
            {
                TemplateId = info.RecruitNotification.EmailTemplateId,
                RecipientAddress = info.RecruitNotification.User.Email,
                Tokens = info.StaticData,
                SourceIds = x.Select(o => o.RecruitNotification.Id).ToList()
            };
        });
    }
}