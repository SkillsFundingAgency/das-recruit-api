using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.AlertsProviderTests;

[TestFixture]
internal class WhenGettingProviderTransferredVacanciesAlertByUkprn
{
    [Test, MoqAutoData]
    public async Task When_User_Not_Found_Should_Return_Empty_Model(int ukprnId,
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
        var result = await sut.GetProviderTransferredVacanciesAlertByUkprn(ukprnId, userId, token);

        // Assert
        result.LegalEntityNames.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_Vacancies_Are_After_LastDismissedDate_Should_Return_Distinct_Sorted_LegalEntityNames(int ukprnId,
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
            new() {
                TransferInfo = MakeTransferInfo("Zeta Ltd",
                    new DateTime(2025,
                        02,
                        01)),
                Status = VacancyStatus.Draft
            },
            new() {
                TransferInfo = MakeTransferInfo("Alpha Ltd",
                    new DateTime(2025,
                        02,
                        01)),
                Status = VacancyStatus.Closed
            },
            new() {
                TransferInfo = MakeTransferInfo("Alpha Ltd",
                    new DateTime(2025,
                        02,
                        01)),
                Status = VacancyStatus.Live
            },
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllTransferInfoByUkprn(ukprnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetProviderTransferredVacanciesAlertByUkprn(ukprnId, userId, token);

        // Assert
        result.LegalEntityNames.Should().BeEquivalentTo(["Alpha Ltd", "Zeta Ltd"],
            options => options.WithStrictOrdering());
    }

    [Test, RecursiveMoqAutoData]
    public async Task When_All_Vacancies_Before_LastDismissedDate_Should_Return_Empty_List(int ukprnId,
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
            new() {
                TransferInfo = MakeTransferInfo("Old Corp",
                    new DateTime(2023,
                        02,
                        01)),
                Status = VacancyStatus.Live
            },
        };

        userRepositoryMock
            .Setup(r => r.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userEntity);

        vacancyRepositoryMock
            .Setup(r => r.GetAllTransferInfoByUkprn(ukprnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetProviderTransferredVacanciesAlertByUkprn(ukprnId, userId, token);

        // Assert
        result.LegalEntityNames.Should().BeEmpty();
    }

    private static string? MakeTransferInfo(string legalEntityName, DateTime transferredDate)
    {
        return ApiUtils.SerializeOrNull<TransferInfo>(new TransferInfo {
            LegalEntityName = legalEntityName,
            TransferredDate = transferredDate
        });
    }
}
