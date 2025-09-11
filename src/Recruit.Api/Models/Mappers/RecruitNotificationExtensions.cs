using System.Text.Json;
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
                StaticData = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.StaticData)!,
                DynamicData = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.DynamicData)!,
            };
    }

    public static IEnumerable<RecruitNotification> ToResponseDto(this IEnumerable<RecruitNotificationEntity>? entity)
    {
        return entity?.Select(x => x.ToResponseDto()!) ?? [];
    }
}