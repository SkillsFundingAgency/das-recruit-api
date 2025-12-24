using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.VacancyRepositoryTests;

public class WhenGettingCountVacanciesByUserId
{
    [Test, MoqAutoData]
    public async Task Then_The_Count_Is_Returned_Based_On_The_SubmittedByUserId_Field(
        Guid userId,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository sut)
    {
        // arrange
        context
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(new List<VacancyEntity>
            {
                new() { Id = Guid.NewGuid(), Status = VacancyStatus.Draft, SubmittedByUserId = userId },
                new() { Id = Guid.NewGuid(), Status = VacancyStatus.Draft, SubmittedByUserId = userId },
                new() { Id = Guid.NewGuid(), Status = VacancyStatus.Draft, SubmittedByUserId = Guid.NewGuid() },
            });
        
        // act
        var result = await sut.CountVacanciesByUserIdAsync(userId, CancellationToken.None);

        // assert
        result.Should().Be(2);
    }
}