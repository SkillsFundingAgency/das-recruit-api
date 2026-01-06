using System.Text.Json;

namespace SFA.DAS.Recruit.Api.Testing.Http;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    
    public static async Task<TEntity?> ReadAsAsync<TEntity>(this HttpContent? content)
    {
        ArgumentNullException.ThrowIfNull(content);

        string json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TEntity>(json, JsonSerializerOptions);
    }
}