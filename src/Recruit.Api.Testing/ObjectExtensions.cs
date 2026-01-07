using System.Text.Json;

namespace SFA.DAS.Recruit.Api.Testing;

public static class ObjectExtensions
{
    public static T JsonClone<T>(this T source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json)!;
    }
}