using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.ApplicationReviewsProviderTests;
[TestFixture]
internal class WhenGettingDashboardCountByUkprn
{
    [Test, MoqAutoData]
    public async Task GettingDashboardByUkprn_Given_Status_Submitted_And_ReviewDate_Null_ShouldReturnDashboard(
        int ukprn,
        int newCount,
        int successfulCount,
        int unsuccessfulCount,
        int sharedCount,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, token))
            .ReturnsAsync([
                new DashboardCountModel {
                    Status = ApplicationReviewStatus.New,
                    Count = newCount
                },
                new DashboardCountModel {
                    Status = ApplicationReviewStatus.Successful,
                    Count = successfulCount
                },
                new DashboardCountModel {
                    Status = ApplicationReviewStatus.Unsuccessful,
                    Count = unsuccessfulCount
                }
            ]);
        // Act
        var result = await provider.GetCountByUkprn(ukprn, token);
        
        // Assert
        result.NewApplicationsCount.Should().Be(newCount);
        result.EmployerReviewedApplicationsCount.Should().Be(0);
        result.UnsuccessfulApplicationsCount.Should().Be(unsuccessfulCount);
        result.SuccessfulApplicationsCount.Should().Be(successfulCount);
        result.SharedApplicationsCount.Should().Be(0);
        repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, token), Times.Once);
    }
}
