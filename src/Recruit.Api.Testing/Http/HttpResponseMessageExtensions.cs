using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Testing.Http;

public static class HttpResponseMessageExtensions
{
    public static async Task<TEntity?> ReadAsAsync<TEntity>(this HttpContent? content)
    {
        ArgumentNullException.ThrowIfNull(content);

        var options = new JsonSerializerOptions 
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        string json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TEntity>(json, options);
    }
}