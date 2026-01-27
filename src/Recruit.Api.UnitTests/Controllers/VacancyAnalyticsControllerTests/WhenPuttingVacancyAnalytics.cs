using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyAnalytics;
using SFA.DAS.Recruit.Api.Models.Responses.VacancyAnalytics;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyAnalyticsControllerTests;
[TestFixture]
internal class WhenPuttingVacancyAnalytics
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_VacancyAnalytics_Is_Created(
        VacancyAnalyticsEntity entity,
        long vacancyReference,
        Mock<IVacancyAnalyticsRepository> repository,
        PutVacancyAnalyticsRequest request,
        [Greedy] VacancyAnalyticsController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyAnalyticsEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, true));

        // act
        var result = await sut.PutOne(vacancyReference, repository.Object, request, token);
        var createdResult = result as Created<VacancyAnalyticsResponse>;
        var payload = createdResult?.Value;

        // assert
        repository.Verify(x => x.UpsertOneAsync(It.IsAny<VacancyAnalyticsEntity>(), token), Times.Once);
        createdResult.Should().NotBeNull();
        createdResult.Location.Should().BeEquivalentTo($"/api/vacancyanalytics/{entity.VacancyReference}");
        payload.Should().BeEquivalentTo(entity, options => options
            .Excluding(x => x.Analytics)
            .Excluding(x => x.AnalyticsData));
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_VacancyAnalytics_Exception_Return_InternalServerException(
        long vacancyReference,
        Mock<IVacancyAnalyticsRepository> repository,
        PutVacancyAnalyticsRequest request,
        [Greedy] VacancyAnalyticsController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyAnalyticsEntity>(), token))
            .ThrowsAsync(new Exception("Boom"));

        // act
        var result = await sut.PutOne(vacancyReference, repository.Object, request, token);

        // assert
        repository.Verify(x => x.UpsertOneAsync(It.IsAny<VacancyAnalyticsEntity>(), token), Times.Once);
        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }
}
