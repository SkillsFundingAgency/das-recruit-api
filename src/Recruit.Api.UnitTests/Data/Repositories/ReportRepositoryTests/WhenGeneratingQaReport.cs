using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

internal class WhenGeneratingQaReport
{
    [Test, RecursiveMoqAutoData]
    public async Task GenerateQa_ReturnsEmpty_WhenReportNotFound(
        Guid reportId,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);

        var result = await sut.GenerateQa(reportId, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task GenerateQa_ReturnsEmpty_WhenReportIsNotQaType(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        entity.OwnerType = ReportOwnerType.Provider;
        entity.CreatedDate = DateTime.UtcNow.AddDays(-1);
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        var result = await sut.GenerateQa(entity.Id, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task GenerateQa_ReturnsEmpty_WhenReportIsExpired(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        entity.OwnerType = ReportOwnerType.Qa;
        entity.CreatedDate = DateTime.UtcNow.AddDays(-8);
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        var result = await sut.GenerateQa(entity.Id, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task GenerateQa_ReturnsEmpty_WhenCriteriaIsInvalid(
        ReportEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        entity.OwnerType = ReportOwnerType.Qa;
        entity.CreatedDate = DateTime.UtcNow.AddDays(-1);
        entity.DynamicCriteria = "not-valid-json";
        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([entity]);

        var result = await sut.GenerateQa(entity.Id, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
