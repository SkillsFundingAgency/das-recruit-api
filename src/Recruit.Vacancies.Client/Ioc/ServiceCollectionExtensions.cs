﻿using System;
using Esfa.Recruit.Vacancies.Client.Application.Cache;
using Esfa.Recruit.Vacancies.Client.Application.CommandHandlers;
using Esfa.Recruit.Vacancies.Client.Application.Configuration;
using Esfa.Recruit.Vacancies.Client.Application.Events;
using Esfa.Recruit.Vacancies.Client.Application.Providers;
using Esfa.Recruit.Vacancies.Client.Application.Queues;
using Esfa.Recruit.Vacancies.Client.Application.Rules.Engine;
using Esfa.Recruit.Vacancies.Client.Application.Services;
using Esfa.Recruit.Vacancies.Client.Application.Validation;
using Esfa.Recruit.Vacancies.Client.Application.Validation.Fluent;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Domain.Messaging;
using Esfa.Recruit.Vacancies.Client.Domain.Repositories;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Client;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Configuration;
using Esfa.Recruit.Vacancies.Client.Infrastructure.EventStore;
using Esfa.Recruit.Vacancies.Client.Infrastructure.HttpRequestHandlers;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Messaging;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Mongo;
using Esfa.Recruit.Vacancies.Client.Infrastructure.OuterApi;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore;
using Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData;
using Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData.ApprenticeshipProgrammes;

using Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData.BannedPhrases;
using Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData.Profanities;
using Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData.Qualifications;
using Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData.Skills;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Repositories;
using Esfa.Recruit.Vacancies.Client.Infrastructure.SequenceStore;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.EmployerAccount;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.ProviderRelationship;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.TrainingProvider;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.TrainingProviderSummaryProvider;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.VacancySummariesProvider;
using Esfa.Recruit.Vacancies.Client.Infrastructure.StorageQueue;
using Esfa.Recruit.Vacancies.Client.Infrastructure.TableStore;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Http.MessageHandlers;
using SFA.DAS.Http.TokenGenerators;
using VacancyRuleSet = Esfa.Recruit.Vacancies.Client.Application.Rules.VacancyRules.VacancyRuleSet;

namespace Esfa.Recruit.Vacancies.Client.Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRecruitStorageClient(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient()
                .Configure<AccountApiConfiguration>(configuration.GetSection("AccountApiConfiguration"))
                .AddMemoryCache()
                .AddTransient<IConfigurationReader, ConfigurationReader>();
            RegisterClients(services);
            RegisterServiceDeps(services, configuration);
            RegisterAccountApiClientDeps(services);
            RegisterTableStorageProviderDeps(services, configuration);
            RegisterRepositories(services, configuration);
            RegisterOutOfProcessEventDelegatorDeps(services, configuration);
            RegisterQueueStorageServices(services, configuration);
            AddValidation(services);
            AddRules(services);
            RegisterMediatR(services);
            RegisterProviderRelationshipsClient(services, configuration);
        }

        private static void RegisterProviderRelationshipsClient(IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ProviderRelationshipsApiConfiguration").Get<ProviderRelationshipApiConfiguration>();
            if (config == null)
            {
                services.AddTransient<IProviderRelationshipsService, ProviderRelationshipsService>();
                return;
            }
            services
                .AddHttpClient<IProviderRelationshipsService, ProviderRelationshipsService>(options =>
                {
                    options.BaseAddress = new Uri(config.ApiBaseUrl);
                })
                .AddHttpMessageHandler(() => new VersionHeaderHandler())
                .AddHttpMessageHandler(() => new ManagedIdentityHeadersHandler(new ManagedIdentityTokenGenerator(config)));
        }

        private static void RegisterAccountApiClientDeps(IServiceCollection services)
        {
            services.AddSingleton<IAccountApiConfiguration>(kernal => kernal.GetService<IOptions<AccountApiConfiguration>>().Value);
            services.AddTransient<IAccountApiClient, AccountApiClient>();
        }

        private static void RegisterServiceDeps(IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            services.AddSingleton(configuration);
            services.Configure<OuterApiConfiguration>(configuration.GetSection("OuterApiConfiguration"));

            // Domain services
            services.AddTransient<ITimeProvider, CurrentUtcTimeProvider>();

            // Application Service
            services.AddTransient<IGenerateVacancyNumbers, MongoSequenceStore>();
            services.AddTransient<ICache, Cache>();
            services.AddTransient<IHtmlSanitizerService, HtmlSanitizerService>();
            services.AddTransient<IEmployerService, EmployerService>();

            // Infrastructure Services
            services.AddTransient<IEmployerAccountProvider, EmployerAccountProvider>();
            services.AddTransient<ITrainingProviderService, TrainingProviderService>();
            services.AddTransient<ITrainingProviderSummaryProvider, TrainingProviderSummaryProvider>();
            services.AddHttpClient<IOuterApiClient, OuterApiClient>();

            // Reference Data Providers
            services.AddTransient<IMinimumWageProvider, NationalMinimumWageProvider>();
            services.AddTransient<IApprenticeshipProgrammeProvider, ApprenticeshipProgrammeProvider>();
            services.AddTransient<IQualificationsProvider, QualificationsProvider>();
            services.AddTransient<ICandidateSkillsProvider, CandidateSkillsProvider>();
            services.AddTransient<IProfanityListProvider, ProfanityListProvider>();
            services.AddTransient<IBannedPhrasesProvider, BannedPhrasesProvider>();

            // Query Data Providers
            services.AddTransient<IVacancySummariesProvider, VacancySummariesProvider>();
            
        }

        private static void RegisterRepositories(IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetConnectionString("MongoDb");

            services.Configure<MongoDbConnectionDetails>(options =>
            {
                options.ConnectionString = mongoConnectionString;
            });

            MongoDbConventions.RegisterMongoConventions();

            //Repositories
            services.AddTransient<IVacancyRepository, MongoDbVacancyRepository>();
            services.AddTransient<IApplicationReviewRepository, MongoDbApplicationReviewRepository>();
            services.AddTransient<IEmployerProfileRepository, MongoDbEmployerProfileRepository>();
            

            //Queries
            services.AddTransient<IVacancyQuery, MongoDbVacancyRepository>();
            services.AddTransient<IBlockedOrganisationQuery, MongoDbBlockedOrganisationRepository>();

            services.AddTransient<IQueryStoreReader, QueryStoreClient>();
            services.AddTransient<IQueryStoreWriter, QueryStoreClient>();

            services.AddTransient<IReferenceDataReader, MongoDbReferenceDataRepository>();
        }

        private static void RegisterOutOfProcessEventDelegatorDeps(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEventStore, StorageQueueEventStore>();
        }

        private static void RegisterQueueStorageServices(IServiceCollection services, IConfiguration configuration)
        {
            var recruitStorageConnectionString = configuration.GetConnectionString("QueueStorage");

            services.AddTransient<IRecruitQueueService>(_ => new RecruitStorageQueueService(recruitStorageConnectionString));
        }

        private static void RegisterTableStorageProviderDeps(IServiceCollection services, IConfiguration configuration)
        {
            var storageConnectionString = configuration.GetConnectionString("TableStorage");
            var useTableStorageQueryStore = configuration.GetValue<bool>("RecruitConfiguration:UseTableStorageQueryStore");
            services.AddTransient<QueryStoreTableChecker>();
            services.Configure<TableStorageConnectionsDetails>(options =>
            {
                options.ConnectionString = storageConnectionString;
            });

            if (useTableStorageQueryStore)
                services.AddTransient<IQueryStore, TableStorageQueryStore>();
            else
            {
                services.AddTransient<IQueryStore, MongoQueryStore>();
            }
        }

        private static void AddValidation(IServiceCollection services)
        {
            services.AddTransient<AbstractValidator<Vacancy>, FluentVacancyValidator>();
            services.AddTransient(typeof(IEntityValidator<,>), typeof(EntityValidator<,>));

            services.AddTransient<AbstractValidator<ApplicationReview>, ApplicationReviewValidator>();
            services.AddTransient<AbstractValidator<VacancyReview>, VacancyReviewValidator>();

            services.AddTransient<AbstractValidator<UserNotificationPreferences>, UserNotificationPreferencesValidator>();
            services.AddTransient<AbstractValidator<Qualification>, QualificationValidator>();
        }

        private static void AddRules(IServiceCollection services)
        {
            services.AddTransient<RuleSet<Vacancy>, VacancyRuleSet>();
        }

        private static void RegisterClients(IServiceCollection services)
        {
            services
                .AddTransient<IRecruitVacancyClient, VacancyClient>()
                .AddTransient<IEmployerVacancyClient, VacancyClient>()
                .AddTransient<IProviderVacancyClient, VacancyClient>();
        }


        private static void RegisterMediatR(IServiceCollection services)
        {
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(CreateEmployerOwnedVacancyCommandHandler).Assembly));
            services
                .AddTransient<IMessaging, MediatrMessaging>();
        }
    }
}
