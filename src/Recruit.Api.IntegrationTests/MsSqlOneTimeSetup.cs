using Microsoft.Extensions.Configuration;
using SFA.DAS.Recruit.Api.Data;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

[SetUpFixture]
public class MsSqlOneTimeSetup
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await using var testServer = new MsSqlTestServer();
        if (testServer.Server.Services.GetService(typeof(RecruitDataContext)) is RecruitDataContext dataContext)
        {
            var configuration = testServer.Server.Services.GetService(typeof(IConfiguration)) as IConfiguration;
            var manager = new TestDataManager(dataContext, null!, configuration.GetDbSchemaName());
            await manager.WipeDataAsync();
        }
    }
}