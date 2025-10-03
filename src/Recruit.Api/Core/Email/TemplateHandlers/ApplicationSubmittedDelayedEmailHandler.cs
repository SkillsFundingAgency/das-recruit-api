using System.Text;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;

public class ApplicationSubmittedDelayedEmailHandler: AbstractEmailHandler
{
    public ApplicationSubmittedDelayedEmailHandler(IEmailTemplateHelper emailTemplateHelper)
    {
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily, UserType.Employer));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly, UserType.Employer));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily, UserType.Provider));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly, UserType.Provider));
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
        var templateGroups = userGroup.GroupBy(x => x.EmailTemplateId);
        return templateGroups.Select(ProcessTemplate);
    }

    private static NotificationEmail ProcessTemplate(IGrouping<Guid, RecruitNotificationEntity> templateGroup)
    {
        // group the records for a user by the vacancy 
        var vacancyGroups = templateGroup
            .Select(x =>
            {
                var dynamicData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(x.DynamicData)!;
                return new {
                    AdvertTitle = dynamicData["advertTitle"],
                    EmployerName = dynamicData["employerName"],
                    Location = dynamicData["location"],
                    ManageVacancyUrl = dynamicData["manageVacancyURL"],
                    RecruitNotification = x,
                    TemplateId = x.EmailTemplateId,
                    VacancyReference = new VacancyReference(dynamicData["vacancyReference"])
                };
            })
            .GroupBy(x => x.VacancyReference)
            .ToList();
        
        // combine each vacancy description into a single string
        var sb = new StringBuilder();
        foreach (var vacancyGroup in vacancyGroups)
        {
            var vacancyDetails = vacancyGroup.First();
            sb.AppendLine($"# {vacancyDetails.AdvertTitle} ({vacancyDetails.VacancyReference.ToString()})");
            sb.AppendLine($"{vacancyDetails.EmployerName}");
            sb.AppendLine($"{vacancyDetails.Location}");
            sb.AppendLine($"[View applications]({vacancyDetails.ManageVacancyUrl}) ({vacancyGroup.Count()} new)");
            sb.AppendLine();
        }

        // build the NotificationEmail
        var details = vacancyGroups.First().First();
        var tokens = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(details.RecruitNotification.StaticData)!;
        tokens.Add("adverts", sb.ToString());

        return new NotificationEmail
        {
            TemplateId = details.TemplateId,
            RecipientAddress = details.RecruitNotification.User.Email,
            Tokens = tokens,
            SourceIds = templateGroup.Select(x => x.Id).ToList()
        };
    }
}