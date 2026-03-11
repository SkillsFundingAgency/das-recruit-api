namespace SFA.DAS.Recruit.Api.Validators;

public interface IExternalWebsiteHealthCheckService
{
    Task<bool> IsHealthyAsync(Uri uri, CancellationToken cancellationToken);
}

public class InvalidSchemeException(string message) : Exception(message);
public class ExternalWebsiteHealthCheckService(ILogger<ExternalWebsiteHealthCheckService> logger, HttpClient httpClient) : IExternalWebsiteHealthCheckService
{
    public async Task<bool> IsHealthyAsync(Uri uri, CancellationToken cancellationToken)
    {
        if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
        {
            throw new InvalidSchemeException("The scheme must be either http or https");
        }

        try
        {
            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            logger.LogInformation("Website validation failed in error state for address {WebsiteUri}", uri);
            return false;
        }
    }
}