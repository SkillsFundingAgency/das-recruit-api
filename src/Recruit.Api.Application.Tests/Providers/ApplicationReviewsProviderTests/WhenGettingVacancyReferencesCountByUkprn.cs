using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Application.Tests.Providers.ApplicationReviewsProviderTests
{
    [TestFixture]
    public class WhenGettingVacancyReferencesCountByUkprn
    {
        [Test, MoqAutoData]
        public async Task GettingVacancyReferencesCountByUkprn_ShouldReturnStats(
            int ukprn,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            var vacancyReferences = new List<long> { 1, 2, 3 };

            var applicationReviews = new List<ApplicationReviewEntity>
            {
                new() { VacancyReference = 1, Status = ApplicationStatus.Submitted.ToString(), ReviewedDate = null },
                new() { VacancyReference = 1, Status = ApplicationStatus.Successful.ToString() },
                new() { VacancyReference = 2, Status = ApplicationStatus.UnSuccessful.ToString() },
                new() { VacancyReference = 3, Status = ApplicationStatus.Submitted.ToString(), ReviewedDate = DateTime.Now },
                new() { VacancyReference = 4, Status = ApplicationStatus.Withdrawn.ToString() }
            };

            repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, vacancyReferences, token))
                .ReturnsAsync(applicationReviews);

            // Act
            var result = await provider.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, token);

            // Assert
            result.Should().HaveCount(4);

            result[0].VacancyReference.Should().Be(1);
            result[0].NewApplications.Should().Be(1);
            result[0].SuccessfulApplications.Should().Be(1);
            result[0].UnsuccessfulApplications.Should().Be(0);
            result[0].Applications.Should().Be(2);

            result[1].VacancyReference.Should().Be(2);
            result[1].NewApplications.Should().Be(0);
            result[1].SuccessfulApplications.Should().Be(0);
            result[1].UnsuccessfulApplications.Should().Be(1);
            result[1].Applications.Should().Be(1);

            result[2].VacancyReference.Should().Be(3);
            result[2].NewApplications.Should().Be(0);
            result[2].SuccessfulApplications.Should().Be(0);
            result[2].UnsuccessfulApplications.Should().Be(0);
            result[2].Applications.Should().Be(1);

            result[3].VacancyReference.Should().Be(4);
            result[3].NewApplications.Should().Be(0);
            result[3].SuccessfulApplications.Should().Be(0);
            result[3].UnsuccessfulApplications.Should().Be(0);
            result[3].Applications.Should().Be(0);

            repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, vacancyReferences, token), Times.Once);
        }
    }
}
