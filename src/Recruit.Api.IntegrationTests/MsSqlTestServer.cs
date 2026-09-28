using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.Encoding;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public class MsSqlTestServer : WebApplicationFactory<Program>
{
    public Mock<IEncodingService> EncodingService { get; } = new ();
    public Mock<IMessageSession> MessageSession { get; } = new ();
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configBuilder =>
            configBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .AddUserSecrets(Assembly.GetExecutingAssembly())
            );

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddTransient<IEncodingService>(_ => EncodingService.Object);
            services.AddTransient<IMessageSession>(_ => MessageSession.Object);
        });
    }
}