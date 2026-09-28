using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public static class HttpContentExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    extension(HttpContent content)
    {
        public async Task<TEntity?> ReadAsAsync<TEntity>()
        {
            ArgumentNullException.ThrowIfNull(content);

            string json = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TEntity>(json, JsonOptions);
        }
    }
}