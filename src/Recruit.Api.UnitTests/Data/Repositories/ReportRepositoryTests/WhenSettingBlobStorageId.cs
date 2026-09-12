using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

internal class WhenSettingBlobStorageId
{
    [Test, RecursiveMoqAutoData]
    public async Task SetBlobStorageIdAsync_SetsBlobStorageId_WhenEntityFound(
        Guid blobId,
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        await sut.SetBlobStorageIdAsync(entity.Id, blobId, CancellationToken.None);

        entity.BlobStorageId.Should().Be(blobId);
        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task SetBlobStorageIdAsync_DoesNotSave_WhenEntityNotFound(
        Guid reportId,
        Guid blobId,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);

        await sut.SetBlobStorageIdAsync(reportId, blobId, CancellationToken.None);

        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
