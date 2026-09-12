using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

internal class WhenGeneratingReport
{
    [Test, RecursiveMoqAutoData]
    public async Task Generate_ReturnsEmpty_WhenReportNotFound(
        Guid reportId,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);

        var result = await sut.Generate(reportId, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Generate_ReturnsEmpty_WhenReportIsExpired(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        entity.CreatedDate = DateTime.UtcNow.AddDays(-8);
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        var result = await sut.Generate(entity.Id, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Generate_ReturnsEmpty_WhenCriteriaIsInvalid(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        entity.CreatedDate = DateTime.UtcNow.AddDays(-1);
        entity.DynamicCriteria = "not-valid-json";
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        var result = await sut.Generate(entity.Id, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
