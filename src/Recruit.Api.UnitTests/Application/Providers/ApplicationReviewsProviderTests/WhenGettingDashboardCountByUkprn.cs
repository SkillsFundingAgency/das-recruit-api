using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests;
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
                new ApplicationReviewsDashboardCountModel {
                    Status = ApplicationReviewStatus.New,
                    Count = newCount
                },
                new ApplicationReviewsDashboardCountModel {
                    Status = ApplicationReviewStatus.Successful,
                    Count = successfulCount
                },
                new ApplicationReviewsDashboardCountModel {
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
