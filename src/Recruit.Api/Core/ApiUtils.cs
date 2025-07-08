using System.Text.Json;
using SFA.DAS.Recruit.Api.Configuration;

namespace SFA.DAS.Recruit.Api.Core;

public static class ApiUtils
{
    public static string? SerializeOrNull<T>(T? value) where T : class
    {
        return value is null ? null : JsonSerializer.Serialize(value, JsonConfig.Options);
    }
    
    public static T? DeserializeOrNull<T>(string? value) where T : class
    {
        return value is null ? null : JsonSerializer.Deserialize<T>(value, JsonConfig.Options);
    }
}