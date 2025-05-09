﻿using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests
{
    [TestFixture]
    internal class WhenGettingDashboardCountByAccountId
    {
        [Test, MoqAutoData]
        public async Task GettingDashboardByAccountId_Given_Status_Submitted_And_ReviewDate_Null_ShouldReturnDashboard(
            long accountId,
            ApplicationReviewStatus status,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            status = ApplicationReviewStatus.New;
            foreach (var entity in entities)
            {
                entity.Status = status.ToString();
                entity.ReviewedDate = null;
                entity.WithdrawnDate = null;
            }
            repositoryMock.Setup(repo => repo.GetAllByAccountId(accountId, status.ToString(), token))
                .ReturnsAsync(entities);
            // Act
            var result = await provider.GetCountByAccountId(accountId, status, token);
            
            // Assert
            result.NewApplicationsCount.Should().Be(entities.Count);
            result.EmployerReviewedApplicationsCount.Should().Be(0);
            repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, status.ToString(), token), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GettingDashboardByAccountId_Given_Status_Submitted_And_ReviewDate__Not_Null_ShouldReturnDashboard(
            long accountId,
            ApplicationReviewStatus status,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            status = ApplicationReviewStatus.Interviewing;
            foreach (var entity in entities)
            {
                entity.Status = status.ToString();
                entity.ReviewedDate = DateTime.Now;
                entity.WithdrawnDate = null;
            }
            repositoryMock.Setup(repo => repo.GetAllByAccountId(accountId, status.ToString(), token))
                .ReturnsAsync(entities);
            // Act
            var result = await provider.GetCountByAccountId(accountId, status, token);

            // Assert
            result.NewApplicationsCount.Should().Be(0);
            result.EmployerReviewedApplicationsCount.Should().Be(entities.Count);
            repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, status.ToString(), token), Times.Once);
        }
    }
}
