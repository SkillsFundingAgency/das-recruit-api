using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Testing.Data;

public static class VacancyReferenceGenerator
{
    private static readonly object LockObject = new();
    private static long _currentValue = 200000000L;

    public static VacancyReference GetNextVacancyReference()
    {
        lock (LockObject)
        {
            return new VacancyReference(_currentValue++);
        }
    }
}