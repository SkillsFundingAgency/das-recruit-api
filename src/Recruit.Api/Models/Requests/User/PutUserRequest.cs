using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Models.Requests.User;

public record PutUserRequest
{
    public string? IdamsUserId { get; set; } 
    [Required] public required UserType? UserType { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required string Email { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastSignedInDate { get; set; }
    public IList<string> EmployerAccountIds { get; set; } = new List<string>();
    public long? Ukprn { get; set; }
    public DateTime? TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn { get; set; }
    public DateTime? ClosedVacanciesBlockedProviderAlertDismissedOn { get; set; }
    public DateTime? TransferredVacanciesBlockedProviderAlertDismissedOn { get; set; }
    public DateTime? ClosedVacanciesWithdrawnByQaAlertDismissedOn { get; set; }
    public NotificationPreferences NotificationPreferences { get; set; } = new();
    public string? DfEUserId { get; set; }
}