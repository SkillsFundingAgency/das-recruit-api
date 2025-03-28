using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Configuration;
using Recruit.Api.Domain.Entities;

namespace Recruit.Api.Data;

public interface IRecruitDataContext
{
    DbSet<ApplicationReviewEntity> ApplicationReviewEntities { get; set; }
    DatabaseFacade Database { get; }
    Task Ping(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}

[ExcludeFromCodeCoverage]
public class RecruitDataContext : DbContext, IRecruitDataContext
{
    public DbSet<ApplicationReviewEntity> ApplicationReviewEntities { get; set; }

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
            
        base.OnModelCreating(modelBuilder);
    }
}