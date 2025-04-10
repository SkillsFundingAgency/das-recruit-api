namespace SFA.DAS.Recruit.Api.Models;

public record EmployerProfile(
    long AccountLegalEntityId,
    long AccountId,
    string? AboutOrganisation,
    string? TradingName,
    List<EmployerProfileAddress> Addresses);