using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Responses.VacancyAnalytics;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyAnalyticsControllerTests;

[TestFixture]
internal class WhenGettingVacancyAnalytics
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_VacancyAnalytics_Is_Returned(
        VacancyAnalyticsEntity entity,
        long vacancyReference,
        Mock<IVacancyAnalyticsRepository> repository,
        [Greedy] VacancyAnalyticsController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(vacancyReference, token))
            .ReturnsAsync(entity);

        // act
        var result = await sut.GetOne(vacancyReference, repository.Object, token);
        var okResult = result as Ok<VacancyAnalyticsResponse>;
        var payload = okResult?.Value;

        // assert
        repository.Verify(x => x.GetOneAsync(It.IsAny<long>(), token), Times.Once);
        okResult.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options
            .Excluding(x => x.Analytics)
            .Excluding(x => x.AnalyticsData));
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_VacancyAnalytics_Not_Found_401_Returned(
        long vacancyReference,
        Mock<IVacancyAnalyticsRepository> repository,
        [Greedy] VacancyAnalyticsController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(vacancyReference, token))
            .ReturnsAsync((VacancyAnalyticsEntity)null!);

        // act
        var result = await sut.GetOne(vacancyReference, repository.Object, token);
        var notResult = result as NotFound;

        // assert
        repository.Verify(x => x.GetOneAsync(It.IsAny<long>(), token), Times.Once);
        notResult.Should().NotBeNull();
        notResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_VacancyAnalytics_Exception_Return_InternalServerException(
        long vacancyReference,
        Mock<IVacancyAnalyticsRepository> repository,
        [Greedy] VacancyAnalyticsController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(vacancyReference, token))
            .ThrowsAsync(new Exception("Boom"));

        // act
        var result = await sut.GetOne(vacancyReference, repository.Object, token);

        // assert
        repository.Verify(x => x.GetOneAsync(It.IsAny<long>(), token), Times.Once);
        var problem = result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }
}
