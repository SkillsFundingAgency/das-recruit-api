using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests;

[TestFixture]
internal class WhenGettingDashboardCountByAccountId
{
    [Test, MoqAutoData]
    public async Task GettingDashboardByAccountId_Given_Status_Submitted_And_ReviewDate_Null_ShouldReturnDashboard(
        long accountId,
        int newCount,
        int successfulCount,
        int unsuccessfulCount,
        int employerUnsuccessfulCount,
        int employerInterviewingCount,
        int sharedCount,
        int allSharedCount,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        repositoryMock.Setup(x => x.GetSharedCountByAccountId(accountId, token)).ReturnsAsync(sharedCount);
        repositoryMock.Setup(x => x.GetAllSharedCountByAccountId(accountId, token)).ReturnsAsync(allSharedCount);
        repositoryMock.Setup(repo => repo.GetAllByAccountId(accountId, token))
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
                },
                new ApplicationReviewsDashboardCountModel {
                    Status = ApplicationReviewStatus.EmployerInterviewing,
                    Count = employerInterviewingCount
                },
                new ApplicationReviewsDashboardCountModel {
                    Status = ApplicationReviewStatus.EmployerUnsuccessful,
                    Count = employerUnsuccessfulCount
                }
            ]);
        // Act
        var result = await provider.GetCountByAccountId(accountId, token);
            
        // Assert
        result.NewApplicationsCount.Should().Be(newCount);
        result.EmployerReviewedApplicationsCount.Should().Be(employerUnsuccessfulCount + employerInterviewingCount);
        result.UnsuccessfulApplicationsCount.Should().Be(unsuccessfulCount);
        result.SuccessfulApplicationsCount.Should().Be(successfulCount);
        result.SharedApplicationsCount.Should().Be(sharedCount);
        result.AllSharedApplicationsCount.Should().Be(allSharedCount);
        repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, token), Times.Once);
    }
        
}