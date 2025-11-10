using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Report;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;
[TestFixture]
internal class WhenPostingReport
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Report_Is_Updated(
        Mock<IReportRepository> repository,
        PostReportRequest request,
        ReportEntity entity,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<ReportEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, false));

        // act
        var result = await sut.Create(repository.Object, request, token);
        var payload = result as Created<Report>;

        // assert
        repository.Verify(x => x.UpsertOneAsync(It.IsAny<ReportEntity>(), token), Times.Once);
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Report_Is_Created(
        Mock<IReportRepository> repository,
        PostReportRequest request,
        ReportEntity entity,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<ReportEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, true));

        // act
        var result = await sut.Create(repository.Object, request, token);
        var createdResult = result as Created<Report>;

        // assert
        repository.Verify(x => x.UpsertOneAsync(It.IsAny<ReportEntity>(), token), Times.Once);
        createdResult.Should().NotBeNull();
        createdResult.Value.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
        createdResult.Location.Should().Be($"/api/reports/{entity.Id}");
    }
}
