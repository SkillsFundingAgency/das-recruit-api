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

    [Test, RecursiveMoqAutoData]
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
        vacancyRepositoryMock.Setup(x => x.GetAllClosedProviderVacanciesByClosureReason(ukprnId, ClosureReason.WithdrawnByQa, It.IsAny<DateTime>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await sut.GetWithDrawnByQaAlertByUkprnId(ukprnId, userId, token);

        result.ClosedVacancies.Should().BeEmpty();
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
        vacancyRepositoryMock.Setup(x => x.GetAllClosedProviderVacanciesByClosureReason(ukprnId, ClosureReason.WithdrawnByQa, user.ClosedVacanciesWithdrawnByQaAlertDismissedOn!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        var result = await sut.GetWithDrawnByQaAlertByUkprnId(ukprnId, userId, token);

        result.ClosedVacancies.Should().ContainInOrder("A Vacancy (VAC1001)", "B Vacancy (VAC1002)");
    }
}
