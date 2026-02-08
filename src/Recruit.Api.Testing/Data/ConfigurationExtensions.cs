using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Recruit.Api.Testing.Data;

public static class ConfigurationExtensions
{
    private const string DatabaseSchemaEnvironmentVariableName = "IntegrationTestsDbSchemaName";
    
    public static string GetDbSchemaName(this IConfiguration? configuration, [CallerMemberName] string callerName = "")
    {
        if (configuration is null)
        {
            throw new Exception($"{callerName} passed a null configuration instance");
        }

        var result = configuration[DatabaseSchemaEnvironmentVariableName];
        return string.IsNullOrEmpty(result)
            ? throw new Exception($"{callerName}: Failed to get integration test database schema name from configuration")
            : result!;
    }
    
}