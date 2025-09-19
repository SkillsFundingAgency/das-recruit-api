using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PatchAsync<T>(this HttpClient client, [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri, JsonPatchDocument<T> patchDocument) where T : class
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(patchDocument);

        // IMPORTANT: System.Json.Text.JsonSerializer does not serialise JsonPatchDocument correctly 
        string stringContent = JsonConvert.SerializeObject(patchDocument);
        var requestContent = new StringContent(stringContent, System.Text.Encoding.UTF8, "application/json-patch+json");
        return client.PatchAsync(requestUri, requestContent);
    }
}