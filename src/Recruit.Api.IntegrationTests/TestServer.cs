using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Data;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public class TestServer : WebApplicationFactory<Program>
{
    public Mock<IRecruitDataContext> DataContext { get; } = new ();
    public Mock<IEncodingService> EncodingService { get; } = new ();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration(configBuilder => configBuilder.SetBasePath(Directory.GetCurrentDirectory()))
            .ConfigureServices(services =>
            {
                services.AddTransient<IRecruitDataContext>(x => DataContext.Object);
                services.AddTransient<IEncodingService>(x => EncodingService.Object);
            })
            .UseEnvironment("IntegrationTest");
        
        return base.CreateHost(builder);
    }
}