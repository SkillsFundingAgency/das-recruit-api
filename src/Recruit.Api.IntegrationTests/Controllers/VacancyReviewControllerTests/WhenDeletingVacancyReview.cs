using System.Net;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyReviewControllerTests;

public class WhenDeletingVacancyReview: BaseFixture
{
    [Test]
    public async Task Then_The_VacancyReview_Is_NotFound()
    {
        // arrange
        Server.DataContext
            .Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyReviewEntity>(10).ToList());

        // act
        var response = await Client.DeleteAsync($"{RouteNames.VacancyReviews}/{Guid.NewGuid()}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task Then_The_VacancyReview_Is_Deleted()
    {
        // arrange
        var items = Fixture.CreateMany<VacancyReviewEntity>(10).ToList();
        var itemToDelete = items[1];
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet(items);

        // act
        var response = await Client.DeleteAsync($"{RouteNames.VacancyReviews}/{itemToDelete.Id}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        Server.DataContext.Verify(x => x.VacancyReviewEntities.Remove(itemToDelete), Times.Once);
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}