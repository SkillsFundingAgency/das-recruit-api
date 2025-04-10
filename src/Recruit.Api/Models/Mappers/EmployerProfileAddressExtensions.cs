using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfileAddress;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class EmployerProfileAddressExtensions
{
    public static EmployerProfileAddress ToResponseDto(this EmployerProfileAddressEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        return new EmployerProfileAddress(
            entity.Id,
            entity.AddressLine1,
            entity.AddressLine2,
            entity.AddressLine3,
            entity.AddressLine4,
            entity.Postcode,
            entity.Latitude,
            entity.Longitude);
    }
    
    public static EmployerProfileAddress ToGetResponse(this EmployerProfileAddressEntity entity)
    {
        return ToResponseDto(entity);
    }
    
    public static IEnumerable<EmployerProfileAddress> ToGetResponse(this IEnumerable<EmployerProfileAddressEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        return entities.Select(x => x.ToResponseDto());
    }
    
    public static EmployerProfileAddress ToPatchResponse(this EmployerProfileAddressEntity entity)
    {
        return ToResponseDto(entity);
    }
    
    public static EmployerProfileAddress ToPostResponse(this EmployerProfileAddressEntity entity)
    {
        return ToResponseDto(entity);
    }

    public static EmployerProfileAddressEntity ToDomain(this PostEmployerProfileAddressRequest request, long accountLegalEntityId)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        
        return new EmployerProfileAddressEntity {
            AccountLegalEntityId = accountLegalEntityId,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            AddressLine3 = request.AddressLine3,
            AddressLine4 = request.AddressLine4,
            Postcode = request.Postcode,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };
    }
}