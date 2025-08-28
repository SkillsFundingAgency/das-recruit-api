using System.ComponentModel.DataAnnotations;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models;

public record RecruitUser
{
    public Guid Id { get; set; }
    [MaxLength(200)] public string? IdamsUserId { get; set; }
    [MaxLength(200)] public string? DfEUserId { get; set; }
    public required UserType UserType { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? LastSignedInDate { get; set; }
    [MaxLength(500)] public required string Name { get; set; }
    [MaxLength(250)] public required string Email { get; set; }
    public List<long> EmployerAccountIds { get; set; } = [];
    public long? Ukprn { get; set; }
    public DateTime? TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn { get; set; }
    public DateTime? ClosedVacanciesBlockedProviderAlertDismissedOn { get; set; }
    public DateTime? TransferredVacanciesBlockedProviderAlertDismissedOn { get; set; }
    public DateTime? ClosedVacanciesWithdrawnByQaAlertDismissedOn { get; set; }
    public NotificationPreferences NotificationPreferences { get; set; } = new();
}