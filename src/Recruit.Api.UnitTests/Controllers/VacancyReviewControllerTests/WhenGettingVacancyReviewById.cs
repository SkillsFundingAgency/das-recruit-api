using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

public class WhenGettingVacancyReviewById
{
    [Test, MoqAutoData]
    public async Task Then_The_Vacancy_Review_Is_Returned(
        ManualQaEditFieldIndicator manualQaEditFieldIndicator,
        VacancyReviewEntity entity,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut)
    {
        // arrange
        var manualQaEditFieldIndicators = new List<ManualQaEditFieldIndicator> { manualQaEditFieldIndicator };
        var json = JsonSerializer.Serialize(manualQaEditFieldIndicators);
        entity.AutomatedQaOutcomeIndicators = "[]";
        entity.ManualQaEditFieldIndicators = json;
        entity.ManualQaFieldIndicators = "[]";
        entity.UpdatedFieldIdentifiers = "[]";
        entity.DismissedAutomatedQaOutcomeIndicators = "[]";

        repository
            .Setup(x => x.GetOneAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        
        // act
        var result = await sut.GetOne(repository.Object, Guid.NewGuid(), CancellationToken.None) as Ok<VacancyReview>;

        // assert
        result.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(entity, options => options
            .Excluding(x => x.AutomatedQaOutcomeIndicators)
            .Excluding(x => x.ManualQaEditFieldIndicators)
            .Excluding(x => x.ManualQaFieldIndicators)
            .Excluding(x => x.UpdatedFieldIdentifiers)
            .Excluding(x => x.DismissedAutomatedQaOutcomeIndicators)
        );
        
        result.Value.ManualQaEditFieldIndicators.Should().BeEquivalentTo(manualQaEditFieldIndicators);
    }
}