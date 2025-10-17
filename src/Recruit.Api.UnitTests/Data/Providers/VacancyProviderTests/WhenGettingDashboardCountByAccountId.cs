using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.VacancyProviderTests;
[TestFixture]
internal class WhenGettingDashboardCountByAccountId
{
    [Test, MoqAutoData]
    public async Task GetCountByAccountId_ShouldReturnCorrectDashboardModel(
        long accountId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Greedy] VacancyProvider vacancyProvider)
    {
        // Arrange
        var now = DateTime.UtcNow;
        var vacancies = new List<VacancyEntity>
        {
            new() { Status = VacancyStatus.Closed },
            new() { Status = VacancyStatus.Draft },
            new() { Status = VacancyStatus.Review },
            new() { Status = VacancyStatus.Referred },
            new() { Status = VacancyStatus.Rejected },
            new() { Status = VacancyStatus.Live, ClosingDate = now.AddDays(3), ApplicationMethod = ApplicationMethod.ThroughFindAnApprenticeship },
            new() { Status = VacancyStatus.Live, ClosingDate = now.AddDays(10), ApplicationMethod = ApplicationMethod.ThroughFindAnApprenticeship },
            new() { Status = VacancyStatus.Submitted },
        };

        var sharedVacancies = new List<VacancyEntity> {
            new() { Status = VacancyStatus.Review },
        };

        vacancyRepositoryMock
            .Setup(r => r.GetAllByAccountId(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await vacancyProvider.GetCountByAccountId(accountId);

        // Assert
        result.ClosedVacanciesCount.Should().Be(1);
        result.DraftVacanciesCount.Should().Be(1);
        result.ReviewVacanciesCount.Should().Be(1);
        result.ReferredVacanciesCount.Should().Be(2); // Referred + Rejected
        result.LiveVacanciesCount.Should().Be(2);
        result.SubmittedVacanciesCount.Should().Be(1);
        result.ClosingSoonVacanciesCount.Should().Be(1); // Only one Live within 5 days
        result.ClosingSoonWithNoApplications.Should().Be(1); // ApplicationMethod == ThroughFindAnApprenticeship
    }

    [Test, MoqAutoData]
    public async Task GetCountByAccountId_ShouldReturnZeroCounts_WhenNoVacancies(
        long accountId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Greedy] VacancyProvider vacancyProvider)
    {
        // Arrange
        vacancyRepositoryMock
            .Setup(r => r.GetAllByAccountId(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await vacancyProvider.GetCountByAccountId(accountId);

        // Assert
        result.ClosedVacanciesCount.Should().Be(0);
        result.DraftVacanciesCount.Should().Be(0);
        result.ReviewVacanciesCount.Should().Be(0);
        result.ReferredVacanciesCount.Should().Be(0);
        result.LiveVacanciesCount.Should().Be(0);
        result.SubmittedVacanciesCount.Should().Be(0);
        result.ClosingSoonVacanciesCount.Should().Be(0);
        result.ClosingSoonWithNoApplications.Should().Be(0);
    }
}
