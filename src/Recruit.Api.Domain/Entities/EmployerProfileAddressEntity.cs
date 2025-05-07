using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class EmployerProfileAddressEntity
{
    [Key]
    public int Id { get; init; }
    public long AccountLegalEntityId { get; init; }
    public required string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? AddressLine3 { get; init; }
    public string? AddressLine4 { get; init; }
    public required string Postcode { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }

    public virtual EmployerProfileEntity EmployerProfile { get; init; } = null!;
}