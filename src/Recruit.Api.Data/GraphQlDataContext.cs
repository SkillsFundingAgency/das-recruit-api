using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.Recruit.Api.Data.Configuration;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data;

public class GraphQlDataContext(IOptions<ConnectionStrings> config, DbContextOptions<GraphQlDataContext> options) : DbContext(options)
{
    public DbSet<VacancyEntity> Vacancies { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = new SqlConnection { ConnectionString = config.Value.SqlConnectionString, };
        optionsBuilder.UseSqlServer(connection, options => options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(20), null));
        
        // Note: useful to keep here
        // optionsBuilder.LogTo(message => Debug.WriteLine(message));
        // optionsBuilder.EnableDetailedErrors();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VacancyEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}