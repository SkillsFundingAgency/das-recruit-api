using System.Net;
using System.Net.Http.Json;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyAnalytics;
using SFA.DAS.Recruit.Api.Models.Responses.VacancyAnalytics;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyAnalyticsControllerTests;
internal class WhenPuttingVacancyAnalytics : BaseFixture
{
    [Test]
    public async Task Then_The_VacancyAnalytics_Is_Added()
    {
        // arrange
        VacancyReference vacancyReference = 99999999;
        Server.DataContext
            .Setup(x => x.VacancyAnalyticsEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyAnalyticsEntity>(10).ToList());

        var request = Fixture.Create<PutVacancyAnalyticsRequest>();

        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.VacancyAnalytics}/{vacancyReference.Value}", request);
        var vacancyReview = await response.Content.ReadAsAsync<VacancyAnalyticsResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        vacancyReview.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());

        Server.DataContext.Verify(x => x.VacancyAnalyticsEntities.AddAsync(It.IsAny<VacancyAnalyticsEntity>(), It.IsAny<CancellationToken>()), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
