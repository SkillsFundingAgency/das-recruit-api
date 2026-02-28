using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Recruit.Api.Models.Requests.BlockedOrganisation;

public record struct PutBlockedOrganisationRequest(
    [property: Required] string OrganisationId,
    [property: Required] string OrganisationType,
    [property: Required] string BlockedStatus,
    [property: Required] string Reason,
    [property: Required] string UpdatedByUserId,
    [property: Required] string UpdatedByUserEmail);
