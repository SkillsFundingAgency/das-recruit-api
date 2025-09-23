using System.Text.Json;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class RecruitNotificationExtensions
{
    public static RecruitNotification? ToResponseDto(this RecruitNotificationEntity? entity)
    {
        return entity is null
            ? null
            : new RecruitNotification {
                Id = entity.Id,
                UserId = entity.UserId,
                EmailTemplateId = entity.EmailTemplateId,
                SendWhen = entity.SendWhen,
                StaticData = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.StaticData, JsonConfig.Options)!,
                DynamicData = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.DynamicData, JsonConfig.Options)!,
            };
    }

    public static IEnumerable<RecruitNotification> ToResponseDto(this IEnumerable<RecruitNotificationEntity>? entities)
    {
        return entities?.Select(x => x.ToResponseDto()!) ?? [];
    }
}