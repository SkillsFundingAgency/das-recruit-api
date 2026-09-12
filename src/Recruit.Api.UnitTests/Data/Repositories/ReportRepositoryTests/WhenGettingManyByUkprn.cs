using System.Text.Json;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ReportRepositoryTests;

[TestFixture]
internal class WhenGettingManyByUkprn
{
    [Test, RecursiveMoqAutoData]
    public async Task GetManyByUkprn_ReturnsReports_MatchingUkprn(
        int ukprn,
        ReportEntity matchingEntity,
        ReportEntity differentUkprnEntity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        matchingEntity.OwnerType = ReportOwnerType.Provider;
        matchingEntity.CreatedDate = DateTime.UtcNow.AddDays(-1);
        matchingEntity.DynamicCriteria = JsonSerializer.Serialize(new ReportCriteria { Ukprn = ukprn });

        differentUkprnEntity.OwnerType = ReportOwnerType.Provider;
        differentUkprnEntity.CreatedDate = DateTime.UtcNow.AddDays(-1);
        differentUkprnEntity.DynamicCriteria = JsonSerializer.Serialize(new ReportCriteria { Ukprn = ukprn + 1 });

        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([matchingEntity, differentUkprnEntity]);

        var result = await sut.GetManyByUkprn(ukprn, CancellationToken.None);

        result.Should().ContainSingle().Which.Id.Should().Be(matchingEntity.Id);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetManyByUkprn_ExcludesReports_OlderThanSevenDays(
        int ukprn,
        ReportEntity recentEntity,
        ReportEntity expiredEntity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        var criteria = JsonSerializer.Serialize(new ReportCriteria { Ukprn = ukprn });

        recentEntity.OwnerType = ReportOwnerType.Provider;
        recentEntity.CreatedDate = DateTime.UtcNow.AddDays(-1);
        recentEntity.DynamicCriteria = criteria;

        expiredEntity.OwnerType = ReportOwnerType.Provider;
        expiredEntity.CreatedDate = DateTime.UtcNow.AddDays(-8);
        expiredEntity.DynamicCriteria = criteria;

        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([recentEntity, expiredEntity]);

        var result = await sut.GetManyByUkprn(ukprn, CancellationToken.None);

        result.Should().ContainSingle().Which.Id.Should().Be(recentEntity.Id);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetManyByUkprn_ExcludesNonProviderReports(
        int ukprn,
        ReportEntity providerEntity,
        ReportEntity qaEntity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        [Greedy] ReportRepository sut)
    {
        var criteria = JsonSerializer.Serialize(new ReportCriteria { Ukprn = ukprn });

        providerEntity.OwnerType = ReportOwnerType.Provider;
        providerEntity.CreatedDate = DateTime.UtcNow.AddDays(-1);
        providerEntity.DynamicCriteria = criteria;

        qaEntity.OwnerType = ReportOwnerType.Qa;
        qaEntity.CreatedDate = DateTime.UtcNow.AddDays(-1);
        qaEntity.DynamicCriteria = criteria;

        dataContext.Setup(x => x.ReportEntities).ReturnsDbSet([providerEntity, qaEntity]);

        var result = await sut.GetManyByUkprn(ukprn, CancellationToken.None);

        result.Should().ContainSingle().Which.Id.Should().Be(providerEntity.Id);
    }
}
