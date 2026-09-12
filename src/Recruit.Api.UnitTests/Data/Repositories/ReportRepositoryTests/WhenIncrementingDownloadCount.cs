using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

internal class WhenIncrementingDownloadCount
{
    [Test, RecursiveMoqAutoData]
    public async Task IncrementReportDownloadCountAsync_IncrementsCount_WhenEntityFound(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        entity.DownloadCount = 3;
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        await sut.IncrementReportDownloadCountAsync(entity.Id, CancellationToken.None);

        entity.DownloadCount.Should().Be(4);
        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task IncrementReportDownloadCountAsync_DoesNotSave_WhenEntityNotFound(
        Guid reportId,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);

        await sut.IncrementReportDownloadCountAsync(reportId, CancellationToken.None);

        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
