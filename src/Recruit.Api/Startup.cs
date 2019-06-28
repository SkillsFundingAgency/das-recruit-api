using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Recruit.Api
{
	public partial class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddAzureTableStorage(
                    options => {
                        options.ConfigurationKeys = new[] { assemblyName };
                        options.EnvironmentNameEnvironmentVariableName = "APPSETTING_ASPNETCORE_Environment";
                        options.StorageConnectionStringEnvironmentVariableName = "APPSETTING_ConfigurationStorageConnectionString";
                        options.PreFixConfigurationKeys = false;
                    }
                )
                .Build();

            HostingEnvironment = env;
        }
    }
}
