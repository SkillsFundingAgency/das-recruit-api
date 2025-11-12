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
            var manager = new TestDataManager(dataContext, null!);
            await manager.WipeDataAsync();
        }
    }
}