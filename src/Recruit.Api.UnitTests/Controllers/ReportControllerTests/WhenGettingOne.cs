using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.Report;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;

[TestFixture]
internal class WhenGettingOne
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_When_BlobStorageId_Is_Set_Report_Is_Downloaded_From_Blob(
        Guid reportId,
        Guid blobId,
        ReportEntity reportEntity,
        GetApplicationReviewReportResponse expectedResponse,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        reportEntity.Id = reportId;
        reportEntity.BlobStorageId = blobId;
        reportEntity.Type = ReportType.ProviderApplications;
        reportEntity.DynamicCriteria = JsonConvert.SerializeObject(new ReportCriteria
        {
            FromDate = DateTime.UtcNow.AddDays(-1),
            ToDate = DateTime.UtcNow.AddDays(1),
            Ukprn = 123456
        });

        var json = System.Text.Json.JsonSerializer.Serialize(expectedResponse, JsonConfig.Options);

        repository
            .Setup(x => x.GetOneAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);
        blobStorageService
            .Setup(x => x.DownloadAsync(blobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // act
        var result = await sut.GetOne(repository.Object, reportId, token);
        var payload = (result as Ok<GetApplicationReviewReportResponse>)?.Value;

        // assert
        blobStorageService.Verify(x => x.DownloadAsync(blobId, It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.Generate(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(expectedResponse);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_When_BlobStorageId_Is_Set_And_Type_Is_Qa_Qa_Report_Is_Downloaded_From_Blob(
        Guid reportId,
        Guid blobId,
        ReportEntity reportEntity,
        GetQaReportResponse expectedResponse,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        reportEntity.Id = reportId;
        reportEntity.BlobStorageId = blobId;
        reportEntity.Type = ReportType.QaApplications;
        reportEntity.DynamicCriteria = JsonConvert.SerializeObject(new ReportCriteria
        {
            FromDate = DateTime.UtcNow.AddDays(-1),
            ToDate = DateTime.UtcNow.AddDays(1)
        });

        var json = System.Text.Json.JsonSerializer.Serialize(expectedResponse, JsonConfig.Options);

        repository
            .Setup(x => x.GetOneAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);
        blobStorageService
            .Setup(x => x.DownloadAsync(blobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // act
        var result = await sut.GetOne(repository.Object, reportId, token);
        var payload = (result as Ok<GetQaReportResponse>)?.Value;

        // assert
        blobStorageService.Verify(x => x.DownloadAsync(blobId, It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.GenerateQa(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(expectedResponse);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_When_BlobStorageId_Is_Null_Falls_Back_To_On_The_Fly_Generation(
        Guid reportId,
        ReportEntity reportEntity,
        List<ApplicationReviewReport> reports,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        reportEntity.Id = reportId;
        reportEntity.BlobStorageId = null;
        reportEntity.Type = ReportType.ProviderApplications;
        reportEntity.DynamicCriteria = JsonConvert.SerializeObject(new ReportCriteria
        {
            FromDate = DateTime.UtcNow.AddDays(-1),
            ToDate = DateTime.UtcNow.AddDays(1),
            Ukprn = 123456
        });

        repository
            .Setup(x => x.GetOneAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);
        repository
            .Setup(x => x.Generate(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reports);

        // act
        var result = await sut.GetOne(repository.Object, reportId, token);
        var payload = (result as Ok<GetApplicationReviewReportResponse>)?.Value;

        // assert
        blobStorageService.Verify(x => x.DownloadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        repository.Verify(x => x.Generate(reportId, It.IsAny<CancellationToken>()), Times.Once());
        payload.Should().NotBeNull();
        payload!.ApplicationReviewReports.Should().BeEquivalentTo(reports, options => options.ExcludingMissingMembers());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_When_BlobStorageId_Is_Null_And_Type_Is_Qa_Falls_Back_To_Qa_Generation(
        Guid reportId,
        ReportEntity reportEntity,
        List<QaReport> reports,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        reportEntity.Id = reportId;
        reportEntity.BlobStorageId = null;
        reportEntity.Type = ReportType.QaApplications;
        reportEntity.DynamicCriteria = JsonConvert.SerializeObject(new ReportCriteria
        {
            FromDate = DateTime.UtcNow.AddDays(-1),
            ToDate = DateTime.UtcNow.AddDays(1)
        });

        repository
            .Setup(x => x.GetOneAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);
        repository
            .Setup(x => x.GenerateQa(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reports);

        // act
        var result = await sut.GetOne(repository.Object, reportId, token);
        var payload = (result as Ok<GetQaReportResponse>)?.Value;

        // assert
        blobStorageService.Verify(x => x.DownloadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        repository.Verify(x => x.GenerateQa(reportId, It.IsAny<CancellationToken>()), Times.Once());
        payload.Should().NotBeNull();
        payload!.QaReports.Should().BeEquivalentTo(reports, options => options.ExcludingMissingMembers());
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
