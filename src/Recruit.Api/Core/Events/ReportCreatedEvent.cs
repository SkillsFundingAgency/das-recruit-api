using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Events;

public sealed record ReportCreatedEvent(Guid ReportId, string UserId, ReportOwnerType OwnerType);
