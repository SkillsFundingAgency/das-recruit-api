using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Responses.Notifications;

public record GetBatchByUserStatusResponse(IEnumerable<NotificationEmail> Emails);