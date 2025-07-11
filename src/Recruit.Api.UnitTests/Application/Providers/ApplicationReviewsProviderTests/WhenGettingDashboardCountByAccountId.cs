using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Enums;

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
                },
                new DashboardCountModel {
                    Status = ApplicationReviewStatus.EmployerInterviewing,
                    Count = employerInterviewingCount
                },
                new DashboardCountModel {
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