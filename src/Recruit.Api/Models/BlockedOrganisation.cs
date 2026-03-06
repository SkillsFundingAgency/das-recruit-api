namespace SFA.DAS.Recruit.Api.Models;

public record BlockedOrganisation(
    Guid Id,
    string OrganisationId,
    string OrganisationType,
    string BlockedStatus,
    string Reason,
    string UpdatedByUserId,
    string UpdatedByUserEmail,
    DateTime UpdatedDate);
