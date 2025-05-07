using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class EmployerProfileEntity
{
    [Key]
    public required long AccountLegalEntityId { get; init; }
    public required long AccountId { get; init; }
    [MaxLength(2000)]
    public string? AboutOrganisation { get; init; }
    [MaxLength(2000)]
    public string? TradingName { get; init; }

    public virtual List<EmployerProfileAddressEntity> Addresses { get; init; } = [];
}