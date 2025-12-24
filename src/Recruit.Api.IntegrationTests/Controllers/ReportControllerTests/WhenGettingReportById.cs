using System.Net;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ReportControllerTests;
internal class WhenGettingReportById : BaseFixture
{
    [Test]
    public async Task Then_The_Report_Is_Returned()
    {
        // arrange
        var items = Fixture.CreateMany<ReportEntity>(10).ToList();
        var expected = items[1];
        Server.DataContext.Setup(x => x.ReportEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.Reports}/{expected.Id}");
        var report = await response.Content.ReadAsAsync<Report>();

        // assert
        response.EnsureSuccessStatusCode();
        report.Should().NotBeNull();
    }

    [Test]
    public async Task Then_The_Report_Is_NotFound()
    {
        // arrange
        Server.DataContext
            .Setup(x => x.ReportEntities)
            .ReturnsDbSet(Fixture.CreateMany<ReportEntity>(10).ToList());

        // act
        var response = await Client.GetAsync($"{RouteNames.Reports}/{Guid.NewGuid()}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}