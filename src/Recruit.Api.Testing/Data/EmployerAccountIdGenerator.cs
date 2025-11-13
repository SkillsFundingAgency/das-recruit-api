namespace SFA.DAS.Recruit.Api.Testing.Data;

public static class EmployerAccountIdGenerator
{
    private static readonly object LockObject = new();
    private static long _currentValue = 100000000L;

    public static long GetNext()
    {
        lock (LockObject)
        {
            return _currentValue++;
        }
    }
}