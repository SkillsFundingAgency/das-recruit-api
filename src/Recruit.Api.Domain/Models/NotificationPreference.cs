using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Models;

public record NotificationPreference(NotificationTypes Event, string Method, NotificationScope Scope, NotificationFrequency Frequency);