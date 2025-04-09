namespace SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;

public record PatchEmployerProfileRequest(
    long? AccountId,
    string? AboutOrganisation,
    string? TradingName);