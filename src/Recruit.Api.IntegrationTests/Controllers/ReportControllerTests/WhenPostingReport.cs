using System.Net;
using System.Net.Http.Json;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.Report;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ReportControllerTests;
internal class WhenPostingReport : BaseFixture
{
    [Test]
    public async Task Then_The_Report_Is_Added()
    {
        // arrange
        var id = Guid.NewGuid();
        Server.DataContext
            .Setup(x => x.ReportEntities)
            .ReturnsDbSet(Fixture.CreateMany<ReportEntity>(10).ToList());

        var request = Fixture.Create<PostReportRequest>();

        // act
        var response = await Client.PostAsJsonAsync($"{RouteNames.Reports}", request);
        var vacancyReview = await response.Content.ReadAsAsync<ReportEntity>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        vacancyReview.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());

        Server.DataContext.Verify(x => x.ReportEntities.AddAsync(It.IsAny<ReportEntity>(), It.IsAny<CancellationToken>()), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}