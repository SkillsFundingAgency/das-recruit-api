using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

[TestFixture]
internal class WhenGettingManyReports
{
    [Test, RecursiveMoqAutoData]
    public async Task GetMany_ReturnsReportsMatchingOwnerType(
        ReportEntity matchingEntity,
        ReportEntity otherEntity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        matchingEntity.OwnerType = ReportOwnerType.Provider;
        otherEntity.OwnerType = ReportOwnerType.Qa;
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([matchingEntity, otherEntity]);

        var result = await sut.GetMany(ReportOwnerType.Provider, CancellationToken.None);

        result.Should().ContainSingle().Which.Id.Should().Be(matchingEntity.Id);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetMany_ReturnsEmpty_WhenNoMatchingOwnerType(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        entity.OwnerType = ReportOwnerType.Qa;
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        var result = await sut.GetMany(ReportOwnerType.Provider, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
