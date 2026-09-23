using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;

[TestFixture]
internal class WhenGettingOne
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_Returns_Report_Entity(
        Guid reportId,
        ReportEntity reportEntity,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        reportEntity.Id = reportId;

        repository
            .Setup(x => x.GetOneAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);

        // act
        var result = await sut.GetOne(repository.Object, reportId, token);
        var payload = (result as Ok<Report>)?.Value;

        // assert
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(reportEntity.Id);
        payload.Name.Should().Be(reportEntity.Name);
        payload.Type.Should().Be(reportEntity.Type);
        payload.OwnerType.Should().Be(reportEntity.OwnerType);
        blobStorageService.Verify(x => x.DownloadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_When_Report_Not_Found_Returns_Not_Found(
        Guid reportId,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReportEntity?)null);

        // act
        var result = await sut.GetOne(repository.Object, reportId, token);

        // assert
        result.Should().BeOfType<NotFound>();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_A_500_Is_Returned_When_An_Exception_Is_Thrown(
        Guid reportId,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(reportId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        // act
        var result = await sut.GetOne(repository.Object, reportId, token);

        // assert
        result.Should().BeOfType<ProblemHttpResult>();
        (result as ProblemHttpResult)!.ProblemDetails.Status.Should().Be(500);
    }
}
