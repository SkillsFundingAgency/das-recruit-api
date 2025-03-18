using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;

namespace SFA.DAS.Recruit.Api
{
    public partial class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            
            services.Configure<AzureActiveDirectoryConfiguration>(Configuration.GetSection("AzureAd"));

            var azureAdConfig = Configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>();
            
            var policies = new Dictionary<string, string>
            {
                {PolicyNames.Default, "Default"},
            };
            services.AddAuthentication(azureAdConfig, policies);

            //services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(CreateApplicationCommand).Assembly));

            //RegisterDasEncodingService(services, Configuration);

            services.AddApplicationInsightsTelemetry(Configuration);
            if (!string.IsNullOrEmpty(Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]!))
            {
                // This service will collect and send telemetry data to Azure Monitor.
                services.AddOpenTelemetry().UseAzureMonitor(options =>
                {
                    options.ConnectionString = Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]!;
                });
            }

            services
                .AddMvc(o =>
                {
                    if (HostingEnvironment.IsDevelopment() == false)
                    {
                        o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                    }
                    o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RecruitAPI", Version = "v1" });
                
            });
        }
        
    }
}