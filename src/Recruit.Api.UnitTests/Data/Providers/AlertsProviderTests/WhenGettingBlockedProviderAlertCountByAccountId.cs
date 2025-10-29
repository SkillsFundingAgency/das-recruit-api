using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.AlertsProviderTests;
[TestFixture]
internal class WhenGettingBlockedProviderAlertCountByAccountId
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
        var result = await sut.GetBlockedProviderAlertCountByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEmpty();
        result.BlockedProviderNames.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_Valid_Blocked_Vacancies_Should_Return_VacancyTitles_And_ProviderNames(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.ClosedVacanciesBlockedProviderAlertDismissedOn = new DateTime(2024, 01, 01);

        var vacancies = new List<VacancyClosureSummaryEntity>
        {
            new() {
                Status = VacancyStatus.Closed,
                ClosedDate = new DateTime(2024, 02, 01),
                ClosureReason = ClosureReason.BlockedByQa,
                Title = "Plumber Apprentice",
                VacancyReference = 1111,
                TransferInfo = MakeTransferInfo("Provider A")
            },
            new() {
                Status = VacancyStatus.Closed,
                ClosedDate = new DateTime(2024, 03, 01),
                ClosureReason = ClosureReason.BlockedByQa,
                Title = "Electrician Apprentice",
                VacancyReference = 2222,
                TransferInfo = MakeTransferInfo("Provider B")
            }
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllClosedEmployerVacanciesByClosureReason(accountId,ClosureReason.BlockedByQa, userEntity.ClosedVacanciesBlockedProviderAlertDismissedOn!.Value, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetBlockedProviderAlertCountByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEquivalentTo("Plumber Apprentice (VAC1111)", "Electrician Apprentice (VAC2222)");
        result.BlockedProviderNames.Should().BeEquivalentTo("Provider A", "Provider B");
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_All_Vacancies_Before_LastDismissedDate_Should_Return_Empty(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.ClosedVacanciesBlockedProviderAlertDismissedOn = new DateTime(2025, 01, 01);

        var vacancies = new List<VacancyEntity>
        {
            new() {
                Status = VacancyStatus.Closed,
                ClosedDate = new DateTime(2024, 01, 01),
                ClosureReason = ClosureReason.BlockedByQa,
                Title = "Old Vacancy",
                VacancyReference = 3333,
                TransferInfo = MakeTransferInfo("Provider X")
            }
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllByAccountId(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetBlockedProviderAlertCountByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEmpty();
        result.BlockedProviderNames.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_Vacancy_Is_Not_Closed_Or_Not_BlockedByQa_Should_Not_Be_Included(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.ClosedVacanciesBlockedProviderAlertDismissedOn = DateTime.MinValue;

        var vacancies = new List<VacancyEntity>
        {
            new VacancyEntity
            {
                Status = VacancyStatus.Live, // not Closed
                ClosedDate = DateTime.UtcNow,
                ClosureReason = ClosureReason.BlockedByQa,
                Title = "Invalid 1",
                VacancyReference = 4444,
                TransferInfo = MakeTransferInfo("Provider A")
            },
            new VacancyEntity
            {
                Status = VacancyStatus.Closed,
                ClosedDate = DateTime.UtcNow,
                ClosureReason = ClosureReason.WithdrawnByQa, // wrong reason
                Title = "Invalid 2",
                VacancyReference = 5555,
                TransferInfo = MakeTransferInfo("Provider B")
            }
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllByAccountId(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetBlockedProviderAlertCountByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEmpty();
        result.BlockedProviderNames.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_ProviderNames_Are_Duplicated_Or_Null_Should_Return_Distinct_Valid_Names(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.ClosedVacanciesBlockedProviderAlertDismissedOn = DateTime.MinValue;

        var vacancies = new List<VacancyClosureSummaryEntity>
        {
            new ()
            {
                Status = VacancyStatus.Closed,
                ClosedDate = DateTime.UtcNow,
                ClosureReason = ClosureReason.BlockedByQa,
                Title = "Job 1",
                VacancyReference = 111,
                TransferInfo = MakeTransferInfo("Provider A")
            },
            new ()
            {
                Status = VacancyStatus.Closed,
                ClosedDate = DateTime.UtcNow,
                ClosureReason = ClosureReason.BlockedByQa,
                Title = "Job 2",
                VacancyReference = 222,
                TransferInfo = MakeTransferInfo("Provider A") // duplicate
            },
            new ()
            {
                Status = VacancyStatus.Closed,
                ClosedDate = DateTime.UtcNow,
                ClosureReason = ClosureReason.BlockedByQa,
                Title = "Job 3",
                VacancyReference = 333,
                TransferInfo = MakeTransferInfo(null) // null provider
            }
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllClosedEmployerVacanciesByClosureReason(accountId,ClosureReason.BlockedByQa, userEntity.ClosedVacanciesBlockedProviderAlertDismissedOn!.Value, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetBlockedProviderAlertCountByAccountId(accountId, userId, token);

        // Assert
        result.BlockedProviderNames.Should().BeEquivalentTo("Provider A");
        result.ClosedVacancies.Should().HaveCount(3); // still collects all ClosedVacancies
    }

    private static string? MakeTransferInfo(string providerName)
    {
        return ApiUtils.SerializeOrNull<TransferInfo>(new TransferInfo {
            ProviderName = providerName
        });
    }
}