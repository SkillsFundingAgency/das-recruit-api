using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

[TestFixture]
internal class WhenGettingOneReport
{
    [Test, RecursiveMoqAutoData]
    public async Task GetOneAsync_ReturnsEntity_WhenFound(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        var result = await sut.GetOneAsync(entity.Id, CancellationToken.None);

        result.Should().BeEquivalentTo(entity);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetOneAsync_ReturnsNull_WhenNotFound(
        Guid id,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);

        var result = await sut.GetOneAsync(id, CancellationToken.None);

        result.Should().BeNull();
    }
}
