using System.Text;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;

public class VacancyFeedbackEmailHandler: AbstractEmailHandler
{
    private readonly IEmailTemplateHelper _emailTemplateHelper;

    public VacancyFeedbackEmailHandler(IEmailTemplateHelper emailTemplateHelper)
    {
        _emailTemplateHelper = emailTemplateHelper;
        SupportedTemplates.Add(emailTemplateHelper.TemplateIds.VacancyFeedbackRequired);
    }
    
    public override IEnumerable<NotificationEmail> CreateNotificationEmails(IEnumerable<RecruitNotificationEntity> recruitNotifications)
    {
        List<NotificationEmail> emails = [];

        // need to potentially morph into a batched email
        var groups = recruitNotifications.GroupBy(x => x.UserId);
        foreach (var group in groups)
        {
            var email = group.Count() > 1
                ? CreateBatchEmail(group)
                : CreateEmail(group);
            emails.Add(email);
        }

        return emails;
    }

    private NotificationEmail CreateBatchEmail(IGrouping<Guid, RecruitNotificationEntity> group)
    {
        var notification = group.First();
        var tokens = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(notification.StaticData) ?? [];

        var advertTitles = group
            .Select(x => (ApiUtils.DeserializeOrNull<Dictionary<string, string>>(x.DynamicData) ?? []).GetValueOrDefault("advertTitle") ?? string.Empty)
            .Where(x => !string.IsNullOrEmpty(x))
            .Order();
        var sb = new StringBuilder();
        foreach (var advertTitle in advertTitles)
        {
            sb.AppendLine(advertTitle);
        }
        tokens.Add("advertTitles", sb.ToString());
        tokens.Remove("feedbackCount");

        var noun = tokens["advertNoun"] switch
        {
            "advert" => "adverts",
            _ => "vacancies"
        };
        tokens["advertNoun"] = noun;
        
        return new NotificationEmail {
            TemplateId = _emailTemplateHelper.TemplateIds.VacancyFeedbackRequiredBatched,
            RecipientAddress = notification.User.Email,
            Tokens = tokens,
            SourceIds = notification.Id > 0 ? [notification.Id] : null
        };
    }

    private static NotificationEmail CreateEmail(IGrouping<Guid, RecruitNotificationEntity> group)
    {
        var notification = group.First();
        var staticData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(notification.StaticData) ?? [];
        var dynamicData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(notification.DynamicData) ?? [];
        var tokens = staticData.Concat(dynamicData).ToDictionary();

        if (tokens.TryGetValue("feedbackCount", out var feedbackCount))
        {
            if (int.TryParse(feedbackCount, out var count))
            {
                tokens["feedbackCount"] = $"{count} applicant{(count == 1 ? "" : "s")}";
            }
        }
        
        return new NotificationEmail {
            TemplateId = notification.EmailTemplateId,
            RecipientAddress = notification.User.Email,
            Tokens = tokens,
            SourceIds = notification.Id > 0 ? [notification.Id] : null
        };
    }
}