using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Recruit.Api.Data;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public class MsSqlTestServer : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder
            .ConfigureHostConfiguration(configBuilder => configBuilder.AddJsonFile("appsettings.Test.json"))
            .ConfigureAppConfiguration(configBuilder => configBuilder.SetBasePath(Directory.GetCurrentDirectory()));

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            var connectionString = context.Configuration.GetConnectionString("Integration");
            
            services.Remove(services.SingleOrDefault(service => typeof(DbContextOptions<RecruitDataContext>) == service.ServiceType)!);
            services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType)!);
            services.AddDbContext<RecruitDataContext>((_, option) => option.UseSqlServer(connectionString));
        });
    }

    public IServiceScope GetServiceScope()
    {
        return Services.CreateScope();
    }
}