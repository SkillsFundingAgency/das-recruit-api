﻿using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests
{
    [TestFixture]
    internal class WhenGettingDashboardCountByUkprn
    {
        [Test, MoqAutoData]
        public async Task GettingDashboardByUkprn_Given_Status_Submitted_And_ReviewDate_Null_ShouldReturnDashboard(
            int ukprn,
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
            repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, token))
                .ReturnsAsync(entities);
            // Act
            var result = await provider.GetCountByUkprn(ukprn, token);
            
            // Assert
            result.NewApplicationsCount.Should().Be(entities.Count);
            result.EmployerReviewedApplicationsCount.Should().Be(0);
            repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, token), Times.Once);
        }

        [Test]
        [MoqInlineAutoData(ApplicationReviewStatus.EmployerUnsuccessful)]
        [MoqInlineAutoData(ApplicationReviewStatus.EmployerInterviewing)]
        public async Task GettingDashboardByUkprn_Given_Status_Submitted_And_ReviewDate__Not_Null_ShouldReturnDashboard(
            ApplicationReviewStatus status,
            int ukprn,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            foreach (var entity in entities)
            {
                entity.Status = status.ToString();
            }
            repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, token))
                .ReturnsAsync(entities);
            // Act
            var result = await provider.GetCountByUkprn(ukprn, token);

            // Assert
            result.EmployerReviewedApplicationsCount.Should().Be(entities.Count);
            repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, token), Times.Once);
        }
    }
}
