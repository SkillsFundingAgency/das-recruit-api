using System.Text.Json;
using SFA.DAS.Recruit.Api.Domain.Configuration;

namespace SFA.DAS.Recruit.Api.Domain.Extensions;

public static class ApiUtils
{
    public static string? SerializeOrNull<T>(T? value) where T : class
    {
        return value is null ? null : JsonSerializer.Serialize(value, JsonConfig.Options);
    }
    
    public static T? DeserializeOrNull<T>(string? value) where T : class
    {
        return string.IsNullOrWhiteSpace(value) ? null : JsonSerializer.Deserialize<T>(value, JsonConfig.Options);
    }
}