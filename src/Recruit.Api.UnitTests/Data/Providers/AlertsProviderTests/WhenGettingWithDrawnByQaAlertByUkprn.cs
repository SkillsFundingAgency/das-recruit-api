using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.AlertsProviderTests;

[TestFixture]
internal class WhenGettingWithDrawnByQaAlertByUkprn
{
    [Test, MoqAutoData]
    public async Task GetWithDrawnByQaAlertByUkprnId_UserNotFound_ReturnsEmptyModel(
        int ukprnId,
        string userId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        userRepositoryMock.Setup(x => x.FindByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        var result = await sut.GetWithDrawnByQaAlertByUkprnId(ukprnId, userId, token);

        result.ClosedVacancies.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task GetWithDrawnByQaAlertByUkprnId_NoVacancies_ReturnsEmptyList(int ukprnId,
        string userId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        var user = new UserEntity {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@test.com",
            UserType = UserType.Employer,
            ClosedVacanciesWithdrawnByQaAlertDismissedOn = null
        };
        userRepositoryMock.Setup(x => x.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        vacancyRepositoryMock.Setup(x => x.GetAllByUkprn(ukprnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await sut.GetWithDrawnByQaAlertByUkprnId(ukprnId, userId, token);

        result.ClosedVacancies.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task GetWithDrawnByQaAlertByUkprnId_FiltersVacanciesByStatusAndClosureReasonAndDate(int ukprnId,
        string userId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        var dismissedDate = new DateTime(2024, 1, 1);
        var user = new UserEntity {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@test.com",
            UserType = UserType.Employer,
            ClosedVacanciesWithdrawnByQaAlertDismissedOn = dismissedDate
        };
        var vacancies = new List<VacancyEntity>
        {
                new() {
                    Id = Guid.NewGuid(),
                    Title = "Vacancy 1",
                    VacancyReference = 1001,
                    Status = VacancyStatus.Closed,
                    ClosureReason = ClosureReason.WithdrawnByQa,
                    ClosedDate = dismissedDate.AddDays(1)
                },
                new() {
                    Id = Guid.NewGuid(),
                    Title = "Vacancy 2",
                    VacancyReference = 1002,
                    Status = VacancyStatus.Closed,
                    ClosureReason = ClosureReason.WithdrawnByQa,
                    ClosedDate = dismissedDate.AddDays(-1) // before dismissed date
                },
                new() {
                    Id = Guid.NewGuid(),
                    Title = "Vacancy 3",
                    VacancyReference = 1003,
                    Status = VacancyStatus.Live,
                    ClosureReason = ClosureReason.WithdrawnByQa,
                    ClosedDate = dismissedDate.AddDays(2)
                },
                new() {
                    Id = Guid.NewGuid(),
                    Title = "Vacancy 4",
                    VacancyReference = 1004,
                    Status = VacancyStatus.Closed,
                    ClosureReason = ClosureReason.BlockedByQa,
                    ClosedDate = dismissedDate.AddDays(2)
                }
            };
        userRepositoryMock.Setup(x => x.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        vacancyRepositoryMock.Setup(x => x.GetAllByUkprn(ukprnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        var result = await sut.GetWithDrawnByQaAlertByUkprnId(ukprnId, userId, token);

        result.ClosedVacancies.Should().ContainSingle()
            .And.Contain("Vacancy 1 (VAC1001)");
    }

    [Test, MoqAutoData]
    public async Task GetWithDrawnByQaAlertByUkprnId_ReturnsOrderedTitles(int ukprnId,
        string userId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        var user = new UserEntity {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@test.com",
            UserType = UserType.Employer,
            ClosedVacanciesWithdrawnByQaAlertDismissedOn = DateTime.MinValue
        };
        var vacancies = new List<VacancyClosureSummaryEntity>
        {
                new() {
                    Title = "B Vacancy",
                    VacancyReference = 1002,
                    ClosureReason = ClosureReason.WithdrawnByQa,
                    ClosedDate = DateTime.UtcNow
                },
                new() {
                    Title = "A Vacancy",
                    VacancyReference = 1001,
                    ClosureReason = ClosureReason.WithdrawnByQa,
                    ClosedDate = DateTime.UtcNow
                }
            };
        userRepositoryMock.Setup(x => x.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        vacancyRepositoryMock.Setup(x => x.GetAllByUkprn(ukprnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        var result = await sut.GetWithDrawnByQaAlertByUkprnId(ukprnId, userId, token);

        result.ClosedVacancies.Should().ContainInOrder("A Vacancy (VAC1001)", "B Vacancy (VAC1002)");
    }
}
