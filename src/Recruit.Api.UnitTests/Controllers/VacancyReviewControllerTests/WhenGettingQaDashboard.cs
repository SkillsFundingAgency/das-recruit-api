using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenGettingQaDashboard
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_QaDashboard_Model_Is_Returned(
        QaDashboard model,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetQaDashboard(It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);

        // act
        var result = await sut.GetQaDashboard(repository.Object, token);
        var payload = (result as Ok<QaDashboard>)?.Value;

        // assert
        repository.Verify(x => x.GetQaDashboard(token), Times.Once());
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(model, options => options.ExcludingMissingMembers());
    }

    [Test, MoqAutoData]
    public async Task Get_ReturnsInternalServerException_WhenException_Thrown(Guid id,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        // Arrange
        repository.Setup(x => x.GetQaDashboard(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var result = await sut.GetQaDashboard(repository.Object, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
