using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.AlertsProviderTests;
[TestFixture]
internal class WhenGettingEmployerTransferredVacanciesAlertByAccountId
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
        var result = await sut.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.EmployerRevokedPermission, token);

        // Assert
        result.TransferredVacanciesProviderNames.Should().BeEmpty();
        result.TransferredVacanciesCount.Should().Be(0);
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_Valid_Transfers_Should_Return_Distinct_Sorted_ProviderNames_And_Count(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn = new DateTime(2024, 01, 01);

        var vacancies = new List<VacancyTransferSummaryEntity>
        {
            new() { TransferInfo = MakeTransferInfo("Provider B", TransferReason.EmployerRevokedPermission, new DateTime(2024, 02, 01)), Status = VacancyStatus.Closed},
            new() { TransferInfo = MakeTransferInfo("Provider A", TransferReason.EmployerRevokedPermission, new DateTime(2024, 03, 01)), Status = VacancyStatus.Closed },
            new() { TransferInfo = MakeTransferInfo("Provider A", TransferReason.EmployerRevokedPermission, new DateTime(2024, 04, 01)), Status = VacancyStatus.Closed }, // duplicate
            new() { TransferInfo = null, Status = VacancyStatus.Closed} // ignored
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllTransferInfoByAccountId(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.EmployerRevokedPermission, token);

        // Assert
        result.TransferredVacanciesProviderNames.Should().BeEquivalentTo(new[] { "Provider A", "Provider B" },
            options => options.WithStrictOrdering());
        result.TransferredVacanciesCount.Should().Be(2);
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_Transfers_Are_Before_LastDismissedDate_Should_Return_Empty(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn = new DateTime(2025, 01, 01);

        var vacancies = new List<VacancyTransferSummaryEntity>
        {
            new() { TransferInfo = MakeTransferInfo("Provider X", TransferReason.EmployerRevokedPermission, new DateTime(2024, 01, 01)), Status = VacancyStatus.Closed}
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllTransferInfoByAccountId(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.EmployerRevokedPermission, token);

        // Assert
        result.TransferredVacanciesProviderNames.Should().BeEmpty();
        result.TransferredVacanciesCount.Should().Be(0);
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_TransferReason_Is_BlockedByQa_Should_Use_Correct_DismissedDate(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.TransferredVacanciesBlockedProviderAlertDismissedOn = new DateTime(2024, 01, 01);

        var vacancies = new List<VacancyTransferSummaryEntity>
        {
            new() { TransferInfo = MakeTransferInfo("Provider Y", TransferReason.BlockedByQa, new DateTime(2024, 02, 01)), Status = VacancyStatus.Approved}
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllTransferInfoByAccountId(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.BlockedByQa, token);

        // Assert
        result.TransferredVacanciesProviderNames.Should().BeEquivalentTo("Provider Y");
        result.TransferredVacanciesCount.Should().Be(1);
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_ProviderNames_Are_Null_Or_Empty_Should_Be_Ignored(long accountId,
        string userId,
        UserEntity userEntity,
        [Frozen] Mock<IVacancyRepository> vacancyRepositoryMock,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Greedy] AlertsProvider sut,
        CancellationToken token)
    {
        // Arrange
        userEntity.TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn = DateTime.MinValue;

        var vacancies = new List<VacancyTransferSummaryEntity>
        {
            new() { TransferInfo = MakeTransferInfo(null, TransferReason.EmployerRevokedPermission, DateTime.UtcNow), Status = VacancyStatus.Closed},
            new() { TransferInfo = MakeTransferInfo("   ", TransferReason.EmployerRevokedPermission, DateTime.UtcNow), Status = VacancyStatus.Approved}
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllTransferInfoByAccountId(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.EmployerRevokedPermission, token);

        // Assert
        result.TransferredVacanciesProviderNames.Should().BeEmpty();
        result.TransferredVacanciesCount.Should().Be(0);
    }

    private static string? MakeTransferInfo(string providerName, TransferReason reason, DateTime transferredDate)
    {
        return ApiUtils.SerializeOrNull<TransferInfo>(new TransferInfo {
            ProviderName = providerName,
            Reason = reason,
            TransferredDate = transferredDate
        });
    }
}
