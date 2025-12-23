using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.VacancyAnalyticsRepositoryTests;
[TestFixture]
internal class WhenUpsertingVacancyAnalytics
{
    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Inserts_New_Entity(
        VacancyAnalyticsEntity entity,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyAnalyticsRepository sut,
        CancellationToken token)
    {
        // arrange
        var dbSet = new List<VacancyAnalyticsEntity>().BuildDbSetMock();
        context.Setup(x => x.VacancyAnalyticsEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(entity, token);

        // assert
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.AddAsync(entity, token), Times.Once);
        result.Created.Should().BeTrue();
    }

    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Updates_Existing_Entity(
        VacancyAnalyticsEntity entity,
        VacancyAnalyticsEntity entity2,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyAnalyticsRepository sut,
        CancellationToken token)
    {
        // arrange
        entity.VacancyReference = entity2.VacancyReference;
        List<VacancyAnalyticsEntity> items = [
            entity2,
        ];
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.VacancyAnalyticsEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(entity, token);

        // assert
        context.Verify(x => x.SetValues(items[0], entity), Times.Once);
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.AddAsync(It.IsAny<VacancyAnalyticsEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        result.Created.Should().BeFalse();
    }
}
