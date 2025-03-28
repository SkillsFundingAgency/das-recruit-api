﻿using Recruit.Api.Application.Providers;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Enums;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Application.Tests.Providers
{
    [TestFixture]
    public class WhenGettingDashboardCountByAccountId
    {
        [Test, MoqAutoData]
        public async Task GettingDashboardByAccountId_Given_Status_Submitted_And_ReviewDate_Null_ShouldReturnDashboard(
            long accountId,
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
                entity.Status = nameof(status);
                entity.ReviewedDate = null;
            }
            repositoryMock.Setup(repo => repo.GetAllByAccountId(accountId, nameof(status), token))
                .ReturnsAsync(entities);
            // Act
            var result = await provider.GetCountByAccountId(accountId, status, token);
            
            // Assert
            result.NewApplicationsCount.Should().Be(entities.Count);
            result.EmployerReviewedApplicationsCount.Should().Be(0);
            repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, nameof(status), token), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GettingDashboardByAccountId_Given_Status_Submitted_And_ReviewDate__Not_Null_ShouldReturnDashboard(
            long accountId,
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
                entity.Status = nameof(status);
                entity.ReviewedDate = DateTime.Now;
            }
            repositoryMock.Setup(repo => repo.GetAllByAccountId(accountId, nameof(status), token))
                .ReturnsAsync(entities);
            // Act
            var result = await provider.GetCountByAccountId(accountId, status, token);

            // Assert
            result.NewApplicationsCount.Should().Be(0);
            result.EmployerReviewedApplicationsCount.Should().Be(entities.Count);
            repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, nameof(status), token), Times.Once);
        }
    }
}
