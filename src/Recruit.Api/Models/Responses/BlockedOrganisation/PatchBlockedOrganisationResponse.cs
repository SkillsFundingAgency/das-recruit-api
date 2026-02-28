namespace SFA.DAS.Recruit.Api.Models.Responses.BlockedOrganisation;

public record struct PatchBlockedOrganisationResponse(
    Guid Id,
    string OrganisationId,
    string OrganisationType,
    string BlockedStatus,
    string Reason,
    string UpdatedByUserId,
    string UpdatedByUserEmail,
    DateTime UpdatedDate);
