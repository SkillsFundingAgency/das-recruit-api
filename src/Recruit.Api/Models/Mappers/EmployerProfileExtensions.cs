using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;
using SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class EmployerProfileExtensions
{
    public static EmployerProfile ToResponseDto(this EmployerProfileEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        
        var addresses = entity.Addresses
            .Select(x => x.ToResponseDto())
            .ToList();
        
        return new EmployerProfile(
            entity.AccountLegalEntityId,
            entity.AccountId,
            entity.AboutOrganisation,
            entity.TradingName,
            addresses);
    }

    public static EmployerProfile ToGetResponse(this EmployerProfileEntity entity)
    {
        return entity.ToResponseDto();
    }

    public static PutEmployerProfileResponse ToPutResponse(this EmployerProfileEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        
        return new PutEmployerProfileResponse(
            entity.AccountLegalEntityId,
            entity.AccountId,
            entity.AboutOrganisation,
            entity.TradingName);
    }

    public static PatchEmployerProfileResponse ToPatchResponse(this EmployerProfileEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        
        return new PatchEmployerProfileResponse {
            AccountLegalEntityId = entity.AccountLegalEntityId,
            AccountId = entity.AccountId,
            AboutOrganisation = entity.AboutOrganisation,
            TradingName = entity.TradingName,
        };
    }

    public static EmployerProfileEntity ToDomain(this PutEmployerProfileRequest request, long accountLegalEntityId)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return new EmployerProfileEntity {
            AboutOrganisation = request.AboutOrganisation,
            AccountId = request.AccountId,
            AccountLegalEntityId = accountLegalEntityId,
            TradingName = request.TradingName,
        };
    }
}