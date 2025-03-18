using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Configuration;
using Recruit.Api.Domain.Entities;

namespace Recruit.Api.Data
{
    public interface IRecruitDataContext
    {
        DbSet<ApplicationReviewEntity> ApplicationReviewEntities { get; set; }
    }

    public class RecruitDataContext : DbContext, IRecruitDataContext
    {
        public DbSet<ApplicationReviewEntity> ApplicationReviewEntities { get; set; }

        private readonly RecruitDatabaseConfiguration? _configuration;
        public RecruitDataContext() {}
        public RecruitDataContext(DbContextOptions options) : base(options) {}
        public RecruitDataContext(IOptions<RecruitDatabaseConfiguration> config, DbContextOptions options) : base(options)
        {
            _configuration = config.Value;
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
}
