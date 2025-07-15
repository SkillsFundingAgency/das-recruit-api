using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Data.ProhibitedContent;
using SFA.DAS.Recruit.Api.Data.Vacancy;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data;

public interface IRecruitDataContext
{
    DbSet<ApplicationReviewEntity> ApplicationReviewEntities { get; }
    DbSet<ProhibitedContentEntity> ProhibitedContentEntities { get; }
    DbSet<EmployerProfileEntity> EmployerProfileEntities { get; }
    DbSet<EmployerProfileAddressEntity> EmployerProfileAddressEntities { get; }
    DbSet<VacancyReviewEntity> VacancyReviewEntities { get; }
    DbSet<VacancyEntity> VacancyEntities { get; }
    DatabaseFacade Database { get; }
    Task Ping(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void SetValues<TEntity>(TEntity to, TEntity from) where TEntity : class;
    Task<VacancyReference> GetNextVacancyReferenceAsync(CancellationToken cancellationToken); 
}

[ExcludeFromCodeCoverage]
internal class RecruitDataContext : DbContext, IRecruitDataContext
{
    public DbSet<ApplicationReviewEntity> ApplicationReviewEntities { get; set; }
    public DbSet<ProhibitedContentEntity> ProhibitedContentEntities { get; set; }
    public DbSet<EmployerProfileEntity> EmployerProfileEntities { get; set; }
    public DbSet<EmployerProfileAddressEntity> EmployerProfileAddressEntities { get; set; }
    public DbSet<VacancyReviewEntity> VacancyReviewEntities { get; set; }
    public DbSet<VacancyEntity> VacancyEntities { get; set; }

    private readonly ConnectionStrings? _configuration;
    public RecruitDataContext() {}
    public RecruitDataContext(DbContextOptions options) : base(options) {}
    public RecruitDataContext(IOptions<ConnectionStrings> config, DbContextOptions options) : base(options)
    {
        _configuration = config.Value;
    }

    public async Task Ping(CancellationToken cancellationToken)
    {
        await Database
            .ExecuteSqlRawAsync("SELECT 1;", cancellationToken)
            .ConfigureAwait(false);
    }

    public void SetValues<TEntity>(TEntity to, TEntity from) where TEntity : class
    {
        Entry(to).CurrentValues.SetValues(from);
    }

    public async Task<VacancyReference> GetNextVacancyReferenceAsync(CancellationToken cancellationToken = default)
    {
        var sqlParameter = new SqlParameter("@result", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
        await Database.ExecuteSqlRawAsync("set @result = next value for VacancyReference", [sqlParameter], cancellationToken);
        return (long)sqlParameter.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();

        var connection = new SqlConnection {
            ConnectionString = _configuration!.SqlConnectionString,
        };

        optionsBuilder.UseSqlServer(connection, options =>
            options.EnableRetryOnFailure(
                5,
                TimeSpan.FromSeconds(20),
                null
            ));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApplicationReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProhibitedContentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EmployerProfileEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EmployerProfileAddressEntityConfiguration());
        modelBuilder.ApplyConfiguration(new VacancyReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new VacancyEntityConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}