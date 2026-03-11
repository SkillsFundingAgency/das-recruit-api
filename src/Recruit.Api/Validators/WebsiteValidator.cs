using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace SFA.DAS.Recruit.Api.Validators;

internal partial class WebsiteValidator<T, TProperty>(IExternalWebsiteHealthCheckService externalWebsiteHealthCheckService) : AsyncPropertyValidator<T, TProperty> 
{
    [GeneratedRegex("^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\\.(?!$)|$)){4}$", RegexOptions.Compiled, matchTimeoutMilliseconds: 500)]
    private static partial Regex IpCheckingRegex();
    
    public override async Task<bool> IsValidAsync(ValidationContext<T> context, TProperty value, CancellationToken cancellation)
    {
        if (value is not string url || !Uri.TryCreate(url, UriKind.Absolute, out var uri) || IpCheckingRegex().IsMatch(uri.DnsSafeHost))
        {
            return false;
        }

        try
        {
            return await externalWebsiteHealthCheckService.IsHealthyAsync(uri, cancellation);
        }
        catch (InvalidSchemeException)
        {
            return false;
        }
    }
    public override string Name => nameof(WebsiteValidator<T, string>);
}