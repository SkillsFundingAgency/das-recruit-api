using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Models.Requests.EmployerProfileAddress;

public record struct PostEmployerProfileAddressRequest(
    [property: Required] string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string? AddressLine4,
    [property: Required] string Postcode,
    double? Latitude,
    double? Longitude);