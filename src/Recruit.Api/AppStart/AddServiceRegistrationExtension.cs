using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Data.ProhibitedContent;
using SFA.DAS.Recruit.Api.Data.Vacancy;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Models.Requests.ApplicationReview;
using SFA.DAS.Recruit.Api.Validators;

namespace SFA.DAS.Recruit.Api.AppStart;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationExtension
{
    public static void AddApplicationDependencies(this IServiceCollection services)
    {
        // validators
        services.AddScoped<IValidator<PutApplicationReviewRequest>, PutApplicationReviewRequestValidator>();

        // providers
        services.AddScoped<IApplicationReviewsProvider, ApplicationReviewsProvider>();

        // repositories
        services.AddScoped<IApplicationReviewRepository, ApplicationReviewRepository>();
        services.AddScoped<IProhibitedContentRepository, ProhibitedContentRepository>();
        services.AddScoped<IEmployerProfileRepository, EmployerProfileRepository>();
        services.AddScoped<IEmployerProfileAddressRepository, EmployerProfileAddressRepository>();
        services.AddScoped<IVacancyReviewRepository, VacancyReviewRepository>();
        services.AddScoped<IVacancyRepository, VacancyRepository>();
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
}