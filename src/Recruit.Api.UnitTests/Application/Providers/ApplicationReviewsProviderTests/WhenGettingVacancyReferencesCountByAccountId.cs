﻿using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests
{
    [TestFixture]
    internal class WhenGettingVacancyReferencesCountByAccountId
    {
        [Test, MoqAutoData]
        public async Task GettingVacancyReferencesCountByAccountId_ShouldReturnStats(
            long accountId,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            var vacancyReferences = new List<long> { 1, 2, 3, 4 };

            var applicationReviews = new List<ApplicationReviewEntity>
            {
                new() { VacancyReference = 1, Status = ApplicationReviewStatus.New.ToString(), ReviewedDate = null, WithdrawnDate = null},
                new() { VacancyReference = 1, Status = ApplicationReviewStatus.Successful.ToString(),WithdrawnDate = null },
                new() { VacancyReference = 2, Status = ApplicationReviewStatus.Unsuccessful.ToString(), WithdrawnDate = null },
                new() { VacancyReference = 3, Status = ApplicationReviewStatus.InReview.ToString(), ReviewedDate = DateTime.Now, WithdrawnDate = null },
                new() { VacancyReference = 4, Status = ApplicationReviewStatus.Interviewing.ToString(), WithdrawnDate = null },
                new() { VacancyReference = 4, Status = ApplicationReviewStatus.Shared.ToString(), WithdrawnDate = null }
            };

            repositoryMock.Setup(repo => repo.GetAllByAccountId(accountId, vacancyReferences, token))
                .ReturnsAsync(applicationReviews);

            // Act
            var result = await provider.GetVacancyReferencesCountByAccountId(accountId, vacancyReferences, token);

            // Assert
            result.Should().HaveCount(4);

            result[0].VacancyReference.Should().Be(1);
            result[0].NewApplications.Should().Be(1);
            result[0].SharedApplications.Should().Be(0);
            result[0].SuccessfulApplications.Should().Be(1);
            result[0].UnsuccessfulApplications.Should().Be(0);
            result[0].Applications.Should().Be(2);

            result[1].VacancyReference.Should().Be(2);
            result[1].NewApplications.Should().Be(0);
            result[1].SharedApplications.Should().Be(0);
            result[1].SuccessfulApplications.Should().Be(0);
            result[1].UnsuccessfulApplications.Should().Be(1);
            result[1].Applications.Should().Be(1);

            result[2].VacancyReference.Should().Be(3);
            result[2].NewApplications.Should().Be(0);
            result[2].SharedApplications.Should().Be(0);
            result[2].SuccessfulApplications.Should().Be(0);
            result[2].UnsuccessfulApplications.Should().Be(0);
            result[2].Applications.Should().Be(1);

            result[3].VacancyReference.Should().Be(4);
            result[3].NewApplications.Should().Be(0);
            result[3].SharedApplications.Should().Be(1);
            result[3].SuccessfulApplications.Should().Be(0);
            result[3].UnsuccessfulApplications.Should().Be(0);
            result[3].Applications.Should().Be(2);

            repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, vacancyReferences, token), Times.Once);
        }
    }
}
