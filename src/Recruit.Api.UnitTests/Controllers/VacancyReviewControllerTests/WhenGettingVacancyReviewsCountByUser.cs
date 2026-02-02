using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenGettingVacancyReviewsCountByUser
{
    [Test, RecruitAutoData]
    public async Task Then_The_Count_Is_Returned(
        string userEmail,
        DateTime assignationExpiry,
        int expectedCount,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        var approvedFirstTime = true;

        repository
            .Setup(x => x.GetCountBySubmittedUserEmail(
                It.IsAny<string>(),
                It.IsAny<bool?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        var result = await sut.GetCountByUser(repository.Object, userEmail, approvedFirstTime, assignationExpiry, token);
        var payload = (result as Ok<int>)?.Value;

        repository.Verify(x => x.GetCountBySubmittedUserEmail(userEmail, approvedFirstTime, assignationExpiry, token), Times.Once);
        payload.Should().Be(expectedCount);
    }

    [Test, RecruitAutoData]
    public async Task Then_Zero_Is_Returned_When_No_Params(
        string userEmal,
        int expectedCount,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        bool? approvedFirstTime = null;
        DateTime? assignationExpiry = null;

        repository
            .Setup(x => x.GetCountBySubmittedUserEmail(
                It.IsAny<string>(),
                It.IsAny<bool?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        var result = await sut.GetCountByUser(repository.Object, userEmal, approvedFirstTime, assignationExpiry, token);
        var payload = (result as Ok<int>)?.Value;

        repository.Verify(x => x.GetCountBySubmittedUserEmail(userEmal, approvedFirstTime, assignationExpiry, token), Times.Once);
        payload.Should().Be(expectedCount);
    }
}
