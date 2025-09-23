using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.ApplicationReview;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Configuration;

namespace SFA.DAS.Recruit.Api.AppStart;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationExtension
{
    public static void AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // validators
        services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

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
        
        // email
        services.AddSingleton(new EmailTemplateHelper(configuration["ResourceEnvironmentName"]));
        services.AddScoped<SharedApplicationEmailStrategy>();
        services.AddScoped<EmployerHasReviewedApplicationEmailStrategy>();
        services.AddScoped<NewApplicationEmailStrategy>();
        services.AddScoped<IApplicationReviewEmailStrategyFactory, ApplicationReviewEmailStrategyFactory>();
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
}