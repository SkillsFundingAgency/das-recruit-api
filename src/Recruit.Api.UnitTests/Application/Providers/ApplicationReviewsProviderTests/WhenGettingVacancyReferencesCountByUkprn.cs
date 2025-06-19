using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests
{
    [TestFixture]
    internal class WhenGettingVacancyReferencesCountByUkprn
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
            var vacancyReferences = new List<long> { 1, 2, 3, 4, 5, 6 };

            var applicationReviews = new List<ApplicationReviewEntity>
            {
                new() { VacancyReference = 1, Status = nameof(ApplicationReviewStatus.New), ReviewedDate = null, WithdrawnDate = null},
                new() { VacancyReference = 1, Status = nameof(ApplicationReviewStatus.Successful),WithdrawnDate = null },
                new() { VacancyReference = 2, Status = nameof(ApplicationReviewStatus.Unsuccessful), WithdrawnDate = null },
                new() { VacancyReference = 3, Status = nameof(ApplicationReviewStatus.InReview), ReviewedDate = DateTime.Now, WithdrawnDate = null },
                new() { VacancyReference = 4, Status = nameof(ApplicationReviewStatus.Interviewing), WithdrawnDate = null },
                new() { VacancyReference = 4, Status = nameof(ApplicationReviewStatus.Shared), WithdrawnDate = null },
                new() { VacancyReference = 5, Status = nameof(ApplicationReviewStatus.Shared), DateSharedWithEmployer = DateTime.Now, WithdrawnDate = null },
            };

            repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, vacancyReferences, token))
                .ReturnsAsync(applicationReviews);

            // Act
            var result = await provider.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, token);

            // Assert
            result.Should().HaveCount(6);

            result[0].VacancyReference.Should().Be(1);
            result[0].NewApplications.Should().Be(1);
            result[0].SharedApplications.Should().Be(0);
            result[0].SuccessfulApplications.Should().Be(1);
            result[0].UnsuccessfulApplications.Should().Be(0);
            result[0].Applications.Should().Be(2);
            result[0].HasNoApplications.Should().BeFalse();

            result[1].VacancyReference.Should().Be(2);
            result[1].NewApplications.Should().Be(0);
            result[1].SharedApplications.Should().Be(0);
            result[1].SuccessfulApplications.Should().Be(0);
            result[1].UnsuccessfulApplications.Should().Be(1);
            result[1].Applications.Should().Be(1);
            result[1].HasNoApplications.Should().BeFalse();

            result[2].VacancyReference.Should().Be(3);
            result[2].NewApplications.Should().Be(0);
            result[1].SharedApplications.Should().Be(0);
            result[2].SuccessfulApplications.Should().Be(0);
            result[2].UnsuccessfulApplications.Should().Be(0);
            result[2].Applications.Should().Be(1);
            result[2].HasNoApplications.Should().BeFalse();

            result[3].VacancyReference.Should().Be(4);
            result[3].NewApplications.Should().Be(0);
            result[3].SharedApplications.Should().Be(1);
            result[3].SuccessfulApplications.Should().Be(0);
            result[3].UnsuccessfulApplications.Should().Be(0);
            result[3].Applications.Should().Be(2);
            result[3].HasNoApplications.Should().BeFalse();

            result[4].VacancyReference.Should().Be(5);
            result[4].NewApplications.Should().Be(0);
            result[4].SharedApplications.Should().Be(1);
            result[4].SuccessfulApplications.Should().Be(0);
            result[4].UnsuccessfulApplications.Should().Be(0);
            result[4].Applications.Should().Be(1);
            result[4].AllSharedApplications.Should().Be(1);
            result[4].HasNoApplications.Should().BeFalse();

            result[5].VacancyReference.Should().Be(6);
            result[5].NewApplications.Should().Be(0);
            result[5].SharedApplications.Should().Be(0);
            result[5].SuccessfulApplications.Should().Be(0);
            result[5].UnsuccessfulApplications.Should().Be(0);
            result[5].Applications.Should().Be(0);
            result[5].AllSharedApplications.Should().Be(0);
            result[5].HasNoApplications.Should().BeTrue();

            repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, vacancyReferences, token), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GettingVacancyReferencesCountByUkprn_Given_WithDrawnDate_ShouldReturnStats(
            int ukprn,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            var vacancyReferences = new List<long> { 1, 2, 3, 4, 5 };

            var applicationReviews = new List<ApplicationReviewEntity>
            {
                new() { VacancyReference = 1, Status = nameof(ApplicationReviewStatus.New), WithdrawnDate = DateTime.Now},
                new() { VacancyReference = 1, Status = nameof(ApplicationReviewStatus.Successful),WithdrawnDate = DateTime.Now },
                new() { VacancyReference = 2, Status = nameof(ApplicationReviewStatus.Unsuccessful), WithdrawnDate = DateTime.Now },
                new() { VacancyReference = 3, Status = nameof(ApplicationReviewStatus.InReview), ReviewedDate = DateTime.Now, WithdrawnDate = DateTime.Now },
                new() { VacancyReference = 4, Status = nameof(ApplicationReviewStatus.Interviewing), WithdrawnDate = DateTime.Now },
                new() { VacancyReference = 4, Status = nameof(ApplicationReviewStatus.Shared), WithdrawnDate = DateTime.Now },
            };

            repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, vacancyReferences, token))
                .ReturnsAsync(applicationReviews);

            // Act
            var result = await provider.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, token);

            // Assert
            result.Should().HaveCount(5);

            result[0].VacancyReference.Should().Be(1);
            result[0].NewApplications.Should().Be(0);
            result[0].SharedApplications.Should().Be(0);
            result[0].SuccessfulApplications.Should().Be(0);
            result[0].UnsuccessfulApplications.Should().Be(0);
            result[0].Applications.Should().Be(0);
            result[0].HasNoApplications.Should().BeTrue();

            result[1].VacancyReference.Should().Be(2);
            result[1].NewApplications.Should().Be(0);
            result[1].SharedApplications.Should().Be(0);
            result[1].SuccessfulApplications.Should().Be(0);
            result[1].UnsuccessfulApplications.Should().Be(0);
            result[1].Applications.Should().Be(0);
            result[1].HasNoApplications.Should().BeTrue();

            result[2].VacancyReference.Should().Be(3);
            result[2].NewApplications.Should().Be(0);
            result[2].SharedApplications.Should().Be(0);
            result[2].SuccessfulApplications.Should().Be(0);
            result[2].UnsuccessfulApplications.Should().Be(0);
            result[2].Applications.Should().Be(0);
            result[2].HasNoApplications.Should().BeTrue();

            result[3].VacancyReference.Should().Be(4);
            result[3].NewApplications.Should().Be(0);
            result[3].SharedApplications.Should().Be(0);
            result[3].SuccessfulApplications.Should().Be(0);
            result[3].UnsuccessfulApplications.Should().Be(0);
            result[3].Applications.Should().Be(0);
            result[3].HasNoApplications.Should().BeTrue();

            result[4].VacancyReference.Should().Be(5);
            result[4].NewApplications.Should().Be(0);
            result[4].SharedApplications.Should().Be(0);
            result[4].SuccessfulApplications.Should().Be(0);
            result[4].UnsuccessfulApplications.Should().Be(0);
            result[4].Applications.Should().Be(0);
            result[4].HasNoApplications.Should().BeTrue();

            repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, vacancyReferences, token), Times.Once);
        }
    }
}
