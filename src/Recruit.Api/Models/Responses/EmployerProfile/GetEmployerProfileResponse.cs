namespace SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

public record struct GetEmployerProfileResponse(
    long AccountLegalEntityId,
    long AccountId,
    string? AboutOrganisation,
    string? TradingName,
    List<Address> Addresses);