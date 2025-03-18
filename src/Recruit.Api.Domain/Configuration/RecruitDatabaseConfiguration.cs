namespace Recruit.Api.Domain.Configuration
{
    public class RecruitDatabaseConfiguration
    {
        public required string ConnectionString { get; set; }
        public required string SqlConnectionString { get; set; }
    }
}
