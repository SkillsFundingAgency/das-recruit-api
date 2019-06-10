using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Recruit.Api.Configuration;

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
                        options.EnvironmentNameEnvironmentVariableName = "ASPNETCORE_Environment";
                        options.StorageConnectionStringEnvironmentVariableName = env.IsDevelopment() ? null : "APPSETTING_ConfigurationStorageConnectionString";
                        options.PreFixConfigurationKeys = false;
                    }
                )
                .Build();

            HostingEnvironment = env;
        }
    }
}
