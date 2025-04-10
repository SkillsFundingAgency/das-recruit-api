using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests
{
    [TestFixture]
    public class WhenGettingDashboardCountByUkprn
    {
        [Test, MoqAutoData]
        public async Task GettingDashboardByUkprn_Given_Status_Submitted_And_ReviewDate_Null_ShouldReturnDashboard(
            int ukprn,
            ApplicationStatus status,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            status = ApplicationStatus.Submitted;
            foreach (var entity in entities)
            {
                entity.Status = status.ToString();
                entity.ReviewedDate = null;
            }
            repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, status.ToString(), token))
                .ReturnsAsync(entities);
            // Act
            var result = await provider.GetCountByUkprn(ukprn, status, token);
            
            // Assert
            result.NewApplicationsCount.Should().Be(entities.Count);
            result.EmployerReviewedApplicationsCount.Should().Be(0);
            repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, status.ToString(), token), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GettingDashboardByUkprn_Given_Status_Submitted_And_ReviewDate__Not_Null_ShouldReturnDashboard(
            int ukprn,
            ApplicationStatus status,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            status = ApplicationStatus.Submitted;
            foreach (var entity in entities)
            {
                entity.Status = status.ToString();
                entity.ReviewedDate = DateTime.Now;
            }
            repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, status.ToString(), token))
                .ReturnsAsync(entities);
            // Act
            var result = await provider.GetCountByUkprn(ukprn, status, token);

            // Assert
            result.NewApplicationsCount.Should().Be(0);
            result.EmployerReviewedApplicationsCount.Should().Be(entities.Count);
            repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, status.ToString(), token), Times.Once);
        }
    }
}
