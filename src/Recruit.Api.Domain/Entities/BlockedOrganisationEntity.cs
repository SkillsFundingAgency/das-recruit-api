using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class BlockedOrganisationEntity
{
    [Key]
    public required Guid Id { get; set; }
    [MaxLength(20)]
    public required string OrganisationId { get; set; }
    [MaxLength(20)]
    public required string OrganisationType { get; init; }
    [MaxLength(20)]
    public required string BlockedStatus { get; init; }
    public required string Reason { get; init; }
    [MaxLength(255)]
    public required string UpdatedByUserId { get; init; }
    [MaxLength(255)]
    public required string UpdatedByUserEmail { get; init; }
    public required DateTime UpdatedDate { get; set; }
}
