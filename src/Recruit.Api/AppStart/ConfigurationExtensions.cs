using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Recruit.Api.AppStart;

public static class ConfigurationExtensions
{
    public static IConfigurationRoot LoadConfiguration(this IConfiguration configuration)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();


        if (configuration.IsLocalOrDevEnvironment())
        {
            configBuilder
                .AddJsonFile("appSettings.json", true)
                .AddJsonFile("appSettings.Development.json", true);

            configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"]?.Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                }
            );
        }

        return configBuilder.Build();
    }

    public static bool IsLocalOrDevEnvironment(this IConfiguration configuration)
    {
        string environmentName = configuration["EnvironmentName"];
        return environmentName!.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
               environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
}