using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Core.Email;

public class RecruitNotificationsResult
{
    public List<RecruitNotificationEntity> Immediate { get; } = [];
    public List<RecruitNotificationEntity> Delayed { get; } = [];
}