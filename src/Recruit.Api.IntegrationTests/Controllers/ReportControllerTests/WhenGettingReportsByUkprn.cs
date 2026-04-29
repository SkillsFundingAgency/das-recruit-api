using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Contracts.ApiRequests;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ReportControllerTests;
internal class WhenGettingReportsByUkprn : BaseFixture
{
    [Test]
    public async Task Then_The_Reports_Are_Returned()
    {
        // arrange
        var items = Fixture.CreateMany<ReportEntity>(10).ToList();
        items[1].DynamicCriteria = JsonSerializer.Serialize(new ReportCriteria {
            Ukprn = 10123
        });
        var expected = items[1];
        Server.DataContext.Setup(x => x.ReportEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync(new GetReportsByUkprnProviderApiRequest((int)expected.Criteria!.Ukprn).GetUrl);
        var reports = await response.Content.ReadAsAsync<List<Report>>();

        // assert
        response.EnsureSuccessStatusCode();
        reports.Should().NotBeNull();
    }
}