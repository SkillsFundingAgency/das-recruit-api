using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.BlockedOrganisation;
using SFA.DAS.Recruit.Api.Models.Responses.BlockedOrganisation;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

internal static class BlockedOrganisationExtensions
{
    private static BlockedOrganisation ToResponseDto(this BlockedOrganisationEntity entity)
    {
        return new BlockedOrganisation(
            entity.Id,
            entity.OrganisationId,
            entity.OrganisationType,
            entity.BlockedStatus,
            entity.Reason,
            entity.UpdatedByUserId,
            entity.UpdatedByUserEmail,
            entity.UpdatedDate);
    }

    public static BlockedOrganisation ToGetResponse(this BlockedOrganisationEntity entity)
    {
        return entity.ToResponseDto();
    }

    public static IEnumerable<BlockedOrganisation> ToGetResponse(this List<BlockedOrganisationEntity> entities)
    {
        return entities.Select(x => x.ToResponseDto());
    }

    public static PutBlockedOrganisationResponse ToPutResponse(this BlockedOrganisationEntity entity)
    {
        return new PutBlockedOrganisationResponse(
            entity.Id,
            entity.OrganisationId,
            entity.OrganisationType,
            entity.BlockedStatus,
            entity.Reason,
            entity.UpdatedByUserId,
            entity.UpdatedByUserEmail,
            entity.UpdatedDate);
    }

    public static PatchBlockedOrganisationResponse ToPatchResponse(this BlockedOrganisationEntity entity)
    {
        return new PatchBlockedOrganisationResponse(
            entity.Id,
            entity.OrganisationId,
            entity.OrganisationType,
            entity.BlockedStatus,
            entity.Reason,
            entity.UpdatedByUserId,
            entity.UpdatedByUserEmail,
            entity.UpdatedDate);
    }

    public static BlockedOrganisationEntity ToDomain(this PutBlockedOrganisationRequest request, Guid id)
    {
        return new BlockedOrganisationEntity
        {
            Id = id,
            OrganisationId = request.OrganisationId,
            OrganisationType = request.OrganisationType,
            BlockedStatus = request.BlockedStatus,
            Reason = request.Reason,
            UpdatedByUserId = request.UpdatedByUserId,
            UpdatedByUserEmail = request.UpdatedByUserEmail,
            UpdatedDate = DateTime.UtcNow
        };
    }
}
