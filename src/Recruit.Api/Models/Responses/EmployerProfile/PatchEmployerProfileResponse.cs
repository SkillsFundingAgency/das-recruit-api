namespace SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

public record struct PatchEmployerProfileResponse(
    long AccountLegalEntityId,
    long AccountId,
    string? AboutOrganisation,
    string? TradingName);
