using Microsoft.Extensions.Diagnostics.HealthChecks;
using Recruit.Api.Data;

namespace SFA.DAS.Recruit.Api.AppStart;

public class DefaultHealthCheck(IRecruitDataContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (dbContext.Database.ProviderName?.EndsWith("InMemory", StringComparison.OrdinalIgnoreCase) ?? false)
        {
            return HealthCheckResult.Healthy();
        }

        try
        {
            await dbContext.Ping(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}