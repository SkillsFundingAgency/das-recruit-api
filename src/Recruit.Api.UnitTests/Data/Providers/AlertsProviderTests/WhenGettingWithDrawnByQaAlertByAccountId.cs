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

        var vacancies = new List<VacancyClosureSummaryEntity>
        {
            new VacancyClosureSummaryEntity
            {
                Status = VacancyStatus.Closed,
                ClosedDate = new DateTime(2024, 02, 01),
                ClosureReason = ClosureReason.WithdrawnByQa,
                Title = "Apprentice Engineer",
                VacancyReference = 1111,
                TransferInfo = "{}"
            },
            new VacancyClosureSummaryEntity
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
            .Setup(r => r.GetAllClosedEmployerVacanciesByClosureReason(accountId,ClosureReason.WithdrawnByQa,userEntity.ClosedVacanciesWithdrawnByQaAlertDismissedOn!.Value,  It.IsAny<CancellationToken>(), VacancyStatus.Closed))
            .ReturnsAsync(vacancies);

        // Act
        var result = await sut.GetWithDrawnByQaAlertByAccountId(accountId, userId, token);

        // Assert
        result.ClosedVacancies.Should().BeEquivalentTo("Apprentice Engineer (VAC1111)", "Software Developer (VAC2222)");
    }
}
