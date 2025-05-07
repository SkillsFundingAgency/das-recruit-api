namespace SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

public record struct PutEmployerProfileResponse(
    long AccountLegalEntityId,
    long AccountId,
    string? AboutOrganisation,
    string? TradingName);