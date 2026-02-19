using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;
using SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Validators;

namespace SFA.DAS.Recruit.Api.AppStart;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationExtension
{
    public static void AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // validators
        services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
        services.AddTransient<IHtmlSanitizerService, HtmlSanitizerService>();
        services.AddTransient<IMinimumWageProvider, MinimumWageProvider>();
        services.AddHttpClient<IExternalWebsiteHealthCheckService, ExternalWebsiteHealthCheckService>();
        services.AddSingleton(TimeProvider.System);

        // providers
        services.AddScoped<IApplicationReviewsProvider, ApplicationReviewsProvider>();
        services.AddScoped<IVacancyProvider, VacancyProvider>();
        services.AddScoped<IAlertsProvider, AlertsProvider>();

        // repositories
        services.AddScoped<IApplicationReviewRepository, ApplicationReviewRepository>();
        services.AddScoped<IProhibitedContentRepository, ProhibitedContentRepository>();
        services.AddScoped<IEmployerProfileRepository, EmployerProfileRepository>();
        services.AddScoped<IEmployerProfileAddressRepository, EmployerProfileAddressRepository>();
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVacancyReviewRepository, VacancyReviewRepository>();
        services.AddScoped<IVacancyRepository, VacancyRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IVacancyAnalyticsRepository, VacancyAnalyticsRepository>();

        // email
        var env = configuration["ResourceEnvironmentName"] ?? "local";
        var isProduction = env.Equals("PRD", StringComparison.CurrentCultureIgnoreCase);
        services.AddSingleton<IRecruitBaseUrls>(isProduction
            ? new ProductionRecruitBaseUrls()
            : new DevelopmentRecruitBaseUrls(env));
        services.AddSingleton<IFaaBaseUrl>(isProduction
            ? new ProductionFaaBaseUrls()
            : new DevelopmentFaaBaseUrls(env));
        services.AddSingleton<IEmailTemplateIds>(isProduction
            ? new ProductionEmailTemplateIds()
            : new DevelopmentEmailTemplateIds());
        
        services.AddSingleton<IEmailTemplateHelper, EmailTemplateHelper>();
        services.AddScoped<ApplicationSharedWithEmployerNotificationFactory>();
        services.AddScoped<SharedApplicationReviewedByEmployerNotificationFactory>();
        services.AddScoped<ApplicationSubmittedNotificationFactory>();
        services.AddScoped<IApplicationReviewNotificationStrategy, ApplicationReviewNotificationStrategy>();
        
        services.AddScoped<VacancyRejectedNotificationFactory>();
        services.AddScoped<VacancySentForReviewNotificationFactory>();
        services.AddScoped<VacancySubmittedNotificationFactory>();
        services.AddScoped<VacancyApprovedNotificationFactory>();
        services.AddScoped<VacancyReferredNotificationFactory>();
        services.AddScoped<IVacancyNotificationStrategy, VacancyNotificationStrategy>();
        
        // email template handlers
        services.AddScoped<IEmailTemplateHandler, StaticDataEmailHandler>();
        services.AddScoped<IEmailTemplateHandler, ApplicationSubmittedDelayedEmailHandler>();
        services.AddScoped<IEmailTemplateHandler, SharedApplicationReviewedByEmployerDelayedEmailHandler>();
        services.AddScoped<IEmailFactory, EmailFactory>();
    }

    public static void AddDatabaseRegistration(
        this IServiceCollection services,
        ConnectionStrings config,
        string? environmentName)
    {
        services.AddHttpContextAccessor();

        if (string.Equals(environmentName, "DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<RecruitDataContext>(options =>
                options.UseInMemoryDatabase("SFA.DAS.Recruit.Api"), ServiceLifetime.Transient);
        }
        else
        {
            services.AddDbContext<RecruitDataContext>(options =>
                options.UseSqlServer(config.SqlConnectionString), ServiceLifetime.Transient);
            services.AddDbContextFactory<GraphQlDataContext>(options => options.UseSqlServer(config.SqlConnectionString), ServiceLifetime.Scoped);
        }

        services.AddScoped<IRecruitDataContext, RecruitDataContext>(provider =>
            provider.GetRequiredService<RecruitDataContext>());
        services.AddScoped(provider =>
            new Lazy<RecruitDataContext>(provider.GetRequiredService<RecruitDataContext>));
    }

    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        // health checks
        services
            .AddHealthChecks()
            .AddCheck<DefaultHealthCheck>("default");
    }
    
    public static void RegisterDasEncodingService(this IServiceCollection services, IConfiguration configuration)
    {
        var dasEncodingConfig = new EncodingConfig { Encodings = [] };
        configuration.GetSection(nameof(dasEncodingConfig.Encodings)).Bind(dasEncodingConfig.Encodings);
        services.AddSingleton(dasEncodingConfig);
        services.AddSingleton<IEncodingService, EncodingService>();
    }

    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder)
    {
        builder.AddTypeExtension(typeof(Data.Queries.VacancyQuery));
        builder.ConfigureSchema(b => b.TryAddRootType(() => new ObjectType(d => d.Name(OperationTypeNames.Query)), HotChocolate.Language.OperationType.Query));
        return builder;
    }
}