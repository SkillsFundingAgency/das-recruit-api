using DtoUserType = SFA.DAS.Recruit.Api.Models.UserType;
using DomainUserType = SFA.DAS.Recruit.Api.Domain.Enums.UserType;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

internal static class UserTypeExtensions
{
    public static DomainUserType ToDomain(this DtoUserType? userType)
    {
        return userType switch {
            DtoUserType.Employer => DomainUserType.Employer,
            DtoUserType.Provider => DomainUserType.Provider,
            _ => throw new ArgumentOutOfRangeException(nameof(userType), userType, null)
        };
    }

    public static DtoUserType ToDto(this DomainUserType userType)
    {
        return userType switch {
            DomainUserType.Employer => DtoUserType.Employer,
            DomainUserType.Provider => DtoUserType.Provider,
            _ => throw new ArgumentOutOfRangeException(nameof(userType), userType, null)
        };
    }
}