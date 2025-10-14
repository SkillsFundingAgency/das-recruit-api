using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyControllerTests;
[TestFixture]
internal class WhenGettingManyClosedVacancies
{
    [Test, RecursiveMoqAutoData]
    public async Task GetManyClosedVacancies_ShouldReturnClosedVacancies_WhenTheyExist(
        long vacancyReference,
        List<VacancyEntity> entities,
        [Frozen] Mock<IVacancyRepository> repository,
        [Greedy] VacancyController controller,
        CancellationToken token)
    {
        // Arrange
        foreach (var vacancyEntity in entities)
        {
            vacancyEntity.TransferInfo = null;
            vacancyEntity.TrainingProvider_Address = null;
            vacancyEntity.Qualifications = null;
            vacancyEntity.ProviderReviewFieldIndicators = null;
            vacancyEntity.EmployerLocations = null;
            vacancyEntity.EmployerReviewFieldIndicators = null;
            vacancyEntity.Skills = null;
            vacancyEntity.VacancyReference = vacancyReference;
            vacancyEntity.Status = VacancyStatus.Closed;
        }
        
        repository.Setup(x => x.GetManyClosedVacanciesByVacancyReferences(new List<long> { vacancyReference }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        // Act
        var result = await controller.GetManyClosedVacanciesByVacancyReferences(repository.Object, 
            new PostClosedVacanciesRequest 
            {
                VacancyReferences = [vacancyReference]
            }, token);

        // Assert
        var payload = (result as Ok<List<Vacancy>>)?.Value;

        payload.Should().NotBeNull();
        var returnedVacancies = payload as List<Vacancy>;
        returnedVacancies.Should().NotBeNull();
        returnedVacancies.Count.Should().Be(3);
        returnedVacancies.All(v => v.VacancyReference == vacancyReference).Should().BeTrue();
        returnedVacancies.All(v => v.Status == VacancyStatus.Closed).Should().BeTrue();
    }
}
