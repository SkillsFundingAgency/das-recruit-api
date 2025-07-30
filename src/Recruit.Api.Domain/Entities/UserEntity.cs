namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string? IdamsUserId { get; set; } 
    public UserType UserType { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? LastSignedInDate { get; set; }
    public string? EmployerAccountIds { get; set; }
    public long? Ukprn { get; set; }
    public DateTime? TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn { get; set; }
    public DateTime? ClosedVacanciesBlockedProviderAlertDismissedOn { get; set; }
    public DateTime? TransferredVacanciesBlockedProviderAlertDismissedOn { get; set; }
    public DateTime? ClosedVacanciesWithdrawnByQaAlertDismissedOn { get; set; }
    public string? DfEUserId { get; set; }
}
public enum UserType
{
    Employer,
    Provider
}