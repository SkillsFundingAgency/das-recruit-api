using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests;
[TestFixture]
internal class WhenGettingDashboardVacanciesCountByAccountId
{
    [Test, MoqAutoData]
    public async Task GetAllByAccountId_ReturnsPaginatedVacancyDetails(
        [Frozen] Mock<IApplicationReviewRepository> repository,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        const long accountId = 123L;
        const int pageNumber = 1;
        const int pageSize = 2;
        var appReviews = new List<ApplicationReviewEntity>
        {
                new() { VacancyReference = 1, Status = nameof(ApplicationReviewStatus.New), WithdrawnDate = null },
                new() { VacancyReference = 1, Status = nameof(ApplicationReviewStatus.New), WithdrawnDate = null },
                new() { VacancyReference = 2, Status = nameof(ApplicationReviewStatus.Shared), WithdrawnDate = null, DateSharedWithEmployer = DateTime.Now}
            };
        var paginated = new PaginatedList<ApplicationReviewEntity>(appReviews, 3, pageNumber, pageSize);
        repository.Setup(r => r.GetPagedByAccountAndStatusAsync(accountId, pageNumber, pageSize, nameof(ApplicationReviewEntity.CreatedDate), false, new List<ApplicationReviewStatus>{ ApplicationReviewStatus.New, ApplicationReviewStatus.Shared }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginated);

        // Act
        var result = await provider.GetAllByAccountId(accountId, pageNumber, pageSize, nameof(ApplicationReviewEntity.CreatedDate), false, [ApplicationReviewStatus.New, ApplicationReviewStatus.Shared]);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(v => v.VacancyReference == 1);
        result.Items.Should().Contain(v => v.VacancyReference == 2);
        var vacancy1 = result.Items.First(v => v.VacancyReference == 1);
        vacancy1.NewApplications.Should().Be(2);
        vacancy1.Applications.Should().Be(2);
        vacancy1.AllSharedApplications.Should().Be(0);
        var vacancy2 = result.Items.First(v => v.VacancyReference == 2);
        vacancy2.NewApplications.Should().Be(0);
        vacancy2.Applications.Should().Be(1);
        vacancy2.AllSharedApplications.Should().Be(1);
    }

    [Test, MoqAutoData]
    public async Task GetAllByAccountId_EmptyList_ReturnsEmptyVacancyDetails(
        [Frozen] Mock<IApplicationReviewRepository> repository,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        const long accountId = 123L;
        const int pageNumber = 1;
        const int pageSize = 10;
        const ApplicationReviewStatus status = ApplicationReviewStatus.New;
        var paginated = new PaginatedList<ApplicationReviewEntity>([], 0, pageNumber, pageSize);
        repository.Setup(r => r.GetPagedByAccountAndStatusAsync(accountId, pageNumber, pageSize, nameof(ApplicationReviewEntity.CreatedDate), false, new List<ApplicationReviewStatus>{status}, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginated);

        // Act
        var result = await provider.GetAllByAccountId(accountId, pageNumber, pageSize, nameof(ApplicationReviewEntity.CreatedDate), false, [status]);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task GetAllByAccountId_CallsRepositoryWithCorrectParameters([Frozen] Mock<IApplicationReviewRepository> repository,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        const long accountId = 456L;
        const int pageNumber = 2;
        const int pageSize = 5;
        string sortColumn = "VacancyTitle";
        const bool isAscending = true;
        const ApplicationReviewStatus status = ApplicationReviewStatus.EmployerInterviewing;
        var paginated = new PaginatedList<ApplicationReviewEntity>([], 0, pageNumber, pageSize);
        repository.Setup(r => r.GetPagedByAccountAndStatusAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, new List<ApplicationReviewStatus>{status}, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginated)
            .Verifiable();

        // Act
        var result = await provider.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, [status]);

        // Assert
        repository.Verify();
        result.Should().NotBeNull();
    }
}
