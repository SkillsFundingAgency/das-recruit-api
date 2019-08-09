using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Recruit.Api.Configuration;
using SFA.DAS.Recruit.Api.Mappers;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api
{
	public partial class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<RecruitConfiguration>(Configuration.GetSection("Recruit"));
            services.Configure<AzureActiveDirectoryConfiguration>(Configuration.GetSection("AzureAd"));

            var serviceProvider = services.BuildServiceProvider();
            var recruitConfig = serviceProvider.GetService<IOptions<RecruitConfiguration>>();

            SetupAuthorization(services, serviceProvider);

            services.AddMediatR(typeof(Startup).Assembly);

            services.AddSingleton<IVacancySummaryMapper, VacancySummaryMapper>();
            services.AddSingleton<IQueryStoreReader, QueryStoreClient>();

            MongoDbConventions.RegisterMongoConventions();

            services.AddHealthChecks()
                    .AddMongoDb(recruitConfig.Value.ConnectionString)
                    .AddApplicationInsightsPublisher();

            services.AddApplicationInsightsTelemetry();

            services
                .AddMvc(o =>
                {
                    if (HostingEnvironment.IsDevelopment() == false)
                    {
                        o.Filters.Add(new AuthorizeFilter("default"));
                    }
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
        }

        private void SetupAuthorization(IServiceCollection services, ServiceProvider serviceProvider)
        {
            if (HostingEnvironment.IsDevelopment() == false)
            {
                var azureActiveDirectoryConfiguration =
                    serviceProvider.GetService<IOptions<AzureActiveDirectoryConfiguration>>();

                services.AddAuthorization(o =>
                {
                    o.AddPolicy("default", policy => { policy.RequireAuthenticatedUser(); });
                });

                services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                    .AddJwtBearer(auth =>
                    {
                        auth.Authority =
                            $"https://login.microsoftonline.com/{azureActiveDirectoryConfiguration.Value.Tenant}";
                        auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidAudiences = new List<string>
                            {
                                azureActiveDirectoryConfiguration.Value.Identifier,
                                azureActiveDirectoryConfiguration.Value.Id
                            }
                        };
                    });

                services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
            }
        }
    }
}