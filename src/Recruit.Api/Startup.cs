using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Asp.Versioning;
using HotChocolate.AspNetCore;
using Microsoft.Extensions.Options;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Api.Common.Middleware;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Recruit.Api.AppStart;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api;

[ExcludeFromCodeCoverage]
internal class Startup
{
    private readonly string _environmentName;
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        _environmentName = configuration["EnvironmentName"]!;

        if (_environmentName == "INTEGRATION")
        {
            Configuration = configuration;
            return;
        }
        
        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddAzureTableStorage(options =>
            {
                options.ConfigurationNameIncludesVersionNumber = true;
                options.ConfigurationKeys = configuration["ConfigNames"]!.Split(",");
                options.EnvironmentName = _environmentName;
                options.PreFixConfigurationKeys = false;
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
            });

#if DEBUG
            config.AddJsonFile("appsettings.Development.json", true);
#endif
            Configuration = config.Build();
    }

    private bool IsEnvironmentLocalOrDev =>
        _environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
        || _environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);

    public void ConfigureServices(IServiceCollection services)
    {
        if (!IsEnvironmentLocalOrDev)
        {
            var azureAdConfiguration = Configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>();

            var policies = new Dictionary<string, string>
            {
                { PolicyNames.Default, "Default" },
            };
            services.AddAuthentication(azureAdConfiguration, policies);
            services
                .AddHealthChecks()
                .AddDbContextCheck<RecruitDataContext>();
        }

        services.Configure<ConnectionStrings>(Configuration.GetSection(nameof(ConnectionStrings)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ConnectionStrings>>()!.Value);
        var connectionStrings = Configuration.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services
            .AddMvc(o =>
            {
                if (!IsEnvironmentLocalOrDev)
                {
                    o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string> {
                        Capacity = 0
                    }));
                }
                o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.RegisterDasEncodingService(Configuration);
        services.AddApplicationDependencies(Configuration);
        services.AddDatabaseRegistration(connectionStrings!, Configuration["EnvironmentName"]);
        services.AddOpenTelemetryRegistration(Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]!);
        services.ConfigureHealthChecks();
        services.AddTransient<VersionHeaderTransformer>();
        services.AddTransient<JsonPatchDocumentTypeTransformer>();
        services.AddTransient<JsonPatchDocumentTransformer>();
        services.AddTransient<HealthChecksTransformer>();
        services.AddTransient<FlagsEnumSchemaTransformer>();
        services.AddTransient<StringEnumSchemaTransformer>();
        services.AddTransient<NumericTypeSchemaTransformer>();
        services.AddEndpointsApiExplorer();
        services.AddOpenApi("swagger", options =>
        {
            options.ShouldInclude = _ => true;
            options.AddOperationTransformer<VersionHeaderTransformer>();
            options.AddOperationTransformer<JsonPatchDocumentTypeTransformer>();
            options.AddDocumentTransformer<JsonPatchDocumentTransformer>();
            options.AddDocumentTransformer<HealthChecksTransformer>();
            options.AddSchemaTransformer<FlagsEnumSchemaTransformer>();
            options.AddSchemaTransformer<StringEnumSchemaTransformer>();
            options.AddSchemaTransformer<NumericTypeSchemaTransformer>();
            options.AddSchemaTransformer((schema, context, _) =>
            {
                if (context.JsonTypeInfo.Type == typeof(VacancyReference))
                {
                    schema.Type = Microsoft.OpenApi.JsonSchemaType.String;
                    schema.Properties?.Clear();
                }
                return Task.CompletedTask;
            });
            options.AddSchemaTransformer((schema, context, _) =>
            {
                if (schema.Properties?.Count > 0)
                    schema.AdditionalPropertiesAllowed = false;
                return Task.CompletedTask;
            });
        });
        services.AddApiVersioning(opt =>
        {
            opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            opt.DefaultApiVersion = new ApiVersion(1, 0);
        });
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services
            .AddGraphQLServer()
            .ModifyCostOptions(options =>
            {
                options.MaxFieldCost = 10_000;
            })
            .AddAuthorization()
            .AddPagingArguments()
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .AddTypes()
            .DisableIntrospection(!IsEnvironmentLocalOrDev)
            .RegisterDbContextFactory<GraphQlDataContext>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseMiddleware<SecurityHeadersMiddleware>();
        
        app.UseAuthentication();
            
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/swagger.json", "SFA.DAS.Recruit.Api v1");
            options.RoutePrefix = string.Empty;
        });
        app.UseHealthChecks();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapOpenApi();
            endpoints.MapControllers();
            var graphQlBuilder = endpoints
                .MapGraphQL()
                .WithOptions(new GraphQLServerOptions {
                    EnableSchemaRequests = IsEnvironmentLocalOrDev,
                    Tool = {
                        Enable = IsEnvironmentLocalOrDev,
                        ServeMode = GraphQLToolServeMode.Embedded
                    }
                });
            
            if (!IsEnvironmentLocalOrDev)
            {
                // TODO: could do with splitting the middleware and applying auth to the endpoint only
                graphQlBuilder.RequireAuthorization();
            }
        });
    }
    
    public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
    {
        serviceProvider.StartNServiceBus(Configuration);
    }
}