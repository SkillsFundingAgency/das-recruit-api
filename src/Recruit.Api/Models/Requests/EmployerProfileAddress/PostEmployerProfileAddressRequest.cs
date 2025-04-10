using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Models.Requests.EmployerProfileAddress;

public record struct PostEmployerProfileAddressRequest(
    [Required] string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string? AddressLine4,
    [Required] string Postcode,
    double? Latitude,
    double? Longitude);