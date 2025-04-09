using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;
using SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class EmployerProfileExtensions
{
    public static EmployerProfileEntity ToDomain(this CreateEmployerProfileRequest request, long accountLegalEntityId)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return new EmployerProfileEntity {
            AboutOrganisation = request.AboutOrganisation,
            AccountId = request.AccountId,
            AccountLegalEntityId = accountLegalEntityId,
            TradingName = request.TradingName,
        };
    }
    
    public static GetEmployerProfileResponse ToGetResponse(this EmployerProfileEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        
        var addresses = entity.Addresses
            .Select(x => new Address {
                Id = x.Id,
                AddressLine1 = x.AddressLine1,
                AddressLine2 = x.AddressLine2,
                AddressLine3 = x.AddressLine3,
                AddressLine4 = x.AddressLine4,
                Postcode = x.Postcode,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
            })
            .ToList();
        
        return new GetEmployerProfileResponse(
            entity.AccountLegalEntityId,
            entity.AccountId,
            entity.AboutOrganisation,
            entity.TradingName,
            addresses);
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

}