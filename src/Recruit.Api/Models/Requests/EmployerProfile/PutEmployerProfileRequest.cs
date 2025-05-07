using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;

public record struct PutEmployerProfileRequest(
    [property: Required] long? AccountId,
    string? AboutOrganisation,
    string? TradingName);