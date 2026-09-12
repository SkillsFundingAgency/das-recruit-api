using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

[TestFixture]
internal class WhenUpsertingReport
{
    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Inserts_WhenIdIsEmpty(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        entity.Id = Guid.Empty;
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);

        var result = await sut.UpsertOneAsync(entity, CancellationToken.None);

        result.Created.Should().BeTrue();
        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Inserts_WhenEntityDoesNotExist(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);

        var result = await sut.UpsertOneAsync(entity, CancellationToken.None);

        result.Created.Should().BeTrue();
        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Updates_WhenEntityExists(
        ReportEntity existing,
        ReportEntity updated,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        updated.Id = existing.Id;
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([existing]);

        var result = await sut.UpsertOneAsync(updated, CancellationToken.None);

        result.Created.Should().BeFalse();
        dataContext.Verify(x => x.SetValues(existing, updated), Times.Once);
        dataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
