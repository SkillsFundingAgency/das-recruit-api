using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

internal static class Measure
{
    private static void CheckTime(long elapsedMilliseconds, long millisecondsTimeout, string callerName)
    {
        if (elapsedMilliseconds > millisecondsTimeout)
        {
            Assert.Fail($"{callerName}: Measured function took {elapsedMilliseconds}ms which is longer than the {millisecondsTimeout}ms timeout specified.");
        }
    }
    
    public static async Task<T> ThisAsync<T>(Func<Task<T>> func, int millisecondsTimeout, [CallerMemberName] string callerName = "")
    {
        var sw = Stopwatch.StartNew();
        var result = await func();
        sw.Stop();
        CheckTime(sw.ElapsedMilliseconds, millisecondsTimeout, callerName);
        return result;
    }

    public static async Task ThisAsync<T>(Func<Task> func, int millisecondsTimeout, [CallerMemberName] string callerName = "")
    {
        var sw = Stopwatch.StartNew();
        await func();
        sw.Stop();
        CheckTime(sw.ElapsedMilliseconds, millisecondsTimeout, callerName);
    }
}