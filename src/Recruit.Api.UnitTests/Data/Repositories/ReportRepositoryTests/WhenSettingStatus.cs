using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

internal class WhenSettingStatus
{
    [Test, RecursiveMoqAutoData]
    public async Task SetStatusAsync_SetsStatus_WhenEntityFound(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        await sut.SetStatusAsync(entity.Id, ReportStatus.Generated, CancellationToken.None);

        entity.Status.Should().Be(ReportStatus.Generated);
        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task SetStatusAsync_DoesNotSave_WhenEntityNotFound(
        Guid reportId,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);

        await sut.SetStatusAsync(reportId, ReportStatus.Failed, CancellationToken.None);

        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
