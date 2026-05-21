using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Core.Extensions;

public static class UserEntityExtensions
{
    public static List<UserEntity> ActiveInTheLastYear(this List<UserEntity> users)
    {
        var cutoff = DateTime.UtcNow.AddYears(-1);
        return users.Where(x => x.LastSignedInDate is not null && x.LastSignedInDate >= cutoff).ToList();
    }
}