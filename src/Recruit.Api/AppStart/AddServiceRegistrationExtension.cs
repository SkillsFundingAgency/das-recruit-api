using System;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Recruit.Api.Application.Providers;
using Recruit.Api.Data;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Recruit.Api.Validators;

namespace SFA.DAS.Recruit.Api.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddApplicationDependencies(this IServiceCollection services)
    {
        // validators
        services.AddScoped<IValidator<ApplicationReviewRequest>, ApplicationReviewRequestValidator>();

        // providers
        services.AddScoped<IApplicationReviewsProvider, ApplicationReviewsProvider>();

        // repositories
        services.AddScoped<IApplicationReviewRepository, ApplicationReviewRepository>();
    }

    public static void AddDatabaseRegistration(
        this IServiceCollection services,
        ConnectionStrings config,
        string environmentName = null)
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