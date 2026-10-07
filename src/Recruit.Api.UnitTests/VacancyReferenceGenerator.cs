using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests;

public static class VacancyReferenceGenerator
{
    private static readonly Lock LockObject = new(); 
    private static long _currentValue = 200000000L;

    public static VacancyReference GetNext()
    {
        lock (LockObject)
        {
            return new VacancyReference(_currentValue++);
        }
    }
}