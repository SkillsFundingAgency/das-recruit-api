using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.VacancyProviderTests;
[TestFixture]
internal class WhenGettingDashboardCountByAccountId
{
    [Test, RecursiveMoqAutoData]
    public async Task GetCountByAccountId_ShouldReturnCorrectDashboardModel(
        long accountId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Greedy] VacancyProvider vacancyProvider)
    {
        // Arrange
        vacancyRepositoryMock
            .Setup(r => r.GetEmployerDashboard(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                    [
                        new VacancyDashboardCountModel { Status = VacancyStatus.Closed, Count = 1 },
                        new VacancyDashboardCountModel { Status = VacancyStatus.Draft, Count = 1 },
                        new VacancyDashboardCountModel { Status = VacancyStatus.Review, Count = 1 },
                        new VacancyDashboardCountModel { Status = VacancyStatus.Referred, Count = 1 },
                        new VacancyDashboardCountModel { Status = VacancyStatus.Rejected, Count = 1 },
                        new VacancyDashboardCountModel { Status = VacancyStatus.Submitted, Count = 1 },
                        new VacancyDashboardCountModel { Status = VacancyStatus.Live, Count = 2 }
                    ]
                );
        vacancyRepositoryMock
            .Setup(x => x.GetEmployerVacanciesClosingSoonWithApplications(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([(10, true), (5, false)]);

        // Act
        var result = await vacancyProvider.GetCountByAccountId(accountId);

        // Assert
        result.ClosedVacanciesCount.Should().Be(1);
        result.DraftVacanciesCount.Should().Be(1);
        result.ReviewVacanciesCount.Should().Be(1);
        result.ReferredVacanciesCount.Should().Be(2); // Referred + Rejected
        result.LiveVacanciesCount.Should().Be(2);
        result.SubmittedVacanciesCount.Should().Be(1);
        result.ClosingSoonVacanciesCount.Should().Be(15); // Only one Live within 5 days
        result.ClosingSoonWithNoApplications.Should().Be(5); // ApplicationMethod == ThroughFindAnApprenticeship
    }

    [Test, MoqAutoData]
    public async Task GetCountByAccountId_ShouldReturnZeroCounts_WhenNoVacancies(
        long accountId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Greedy] VacancyProvider vacancyProvider)
    {
        // Arrange
        vacancyRepositoryMock
            .Setup(r => r.GetEmployerDashboard(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        vacancyRepositoryMock
            .Setup(x => x.GetEmployerVacanciesClosingSoonWithApplications(accountId, It.IsAny<CancellationToken>()))
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
