using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;

public record struct PutEmployerProfileRequest(
    [Required] long AccountId,
    string? AboutOrganisation,
    string? TradingName);