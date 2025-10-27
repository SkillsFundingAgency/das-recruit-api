using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.AlertsProviderTests;
[TestFixture]
internal class WhenGettingWithDrawnByQaAlertByAccountId
{
    [Test, MoqAutoData]
    public async Task When_User_Not_Found_Should_Return_Empty_Model(long accountId,
        string userId,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null!);

        // Act
        var result = await sut.GetWithDrawnByQaAlertByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_Vacancies_After_LastDismissedDate_And_WithdrawnByQa_Should_Return_VacancyTitles(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.ClosedVacanciesWithdrawnByQaAlertDismissedOn = new DateTime(2024, 01, 01);

        var vacancies = new List<VacancyEntity>
        {
            new VacancyEntity
            {
                Status = VacancyStatus.Closed,
                ClosedDate = new DateTime(2024, 02, 01),
                ClosureReason = ClosureReason.WithdrawnByQa,
                Title = "Apprentice Engineer",
                VacancyReference = 1111,
                TransferInfo = "{}"
            },
            new VacancyEntity
            {
                Status = VacancyStatus.Closed,
                ClosedDate = new DateTime(2024, 03, 01),
                ClosureReason = ClosureReason.WithdrawnByQa,
                Title = "Software Developer",
                VacancyReference = 2222,
                TransferInfo = "{}"
            }
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllClosedEmployerVacanciesByClosureReason(accountId,ClosureReason.WithdrawnByQa,userEntity.ClosedVacanciesWithdrawnByQaAlertDismissedOn!.Value,  It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetWithDrawnByQaAlertByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEquivalentTo("Apprentice Engineer (VAC1111)", "Software Developer (VAC2222)");
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_All_Vacancies_Before_LastDismissedDate_Should_Return_Empty_List(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.ClosedVacanciesWithdrawnByQaAlertDismissedOn = new DateTime(2025, 01, 01);

        var vacancies = new List<VacancyEntity>
        {
            new VacancyEntity
            {
                Status = VacancyStatus.Closed,
                ClosedDate = new DateTime(2024, 01, 01),
                ClosureReason = ClosureReason.WithdrawnByQa,
                Title = "Old Job",
                VacancyReference = 3333
            }
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllClosedEmployerVacanciesByClosureReason(accountId,ClosureReason.WithdrawnByQa,userEntity.ClosedVacanciesWithdrawnByQaAlertDismissedOn!.Value,  It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetWithDrawnByQaAlertByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_Vacancy_Is_Not_Closed_Or_Reason_Is_Not_WithdrawnByQa_Should_Not_Be_Included(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.
            ClosedVacanciesWithdrawnByQaAlertDismissedOn = DateTime.MinValue;

        var vacancies = new List<VacancyEntity>
        {
            new VacancyEntity
            {
                Status = VacancyStatus.Live, // not Closed
                ClosedDate = DateTime.UtcNow,
                ClosureReason = ClosureReason.WithdrawnByQa,
                Title = "Invalid Vacancy",
                VacancyReference = 4444
            },
            new VacancyEntity
            {
                Status = VacancyStatus.Closed,
                ClosedDate = DateTime.UtcNow,
                ClosureReason = ClosureReason.TransferredByEmployer, // wrong reason
                Title = "Another Invalid Vacancy",
                VacancyReference = 5555
            }
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllClosedEmployerVacanciesByClosureReason(accountId,ClosureReason.WithdrawnByQa,userEntity.ClosedVacanciesWithdrawnByQaAlertDismissedOn!.Value,  It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetWithDrawnByQaAlertByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEmpty();
    }
}
