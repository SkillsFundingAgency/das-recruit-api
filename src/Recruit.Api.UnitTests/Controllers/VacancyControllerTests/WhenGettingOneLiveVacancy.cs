using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyControllerTests;
[TestFixture]
internal class WhenGettingOneLiveVacancy
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Vacancy_Entity_Is_Returned(
        long vacancyReference,
        VacancyEntity entity,
        Mock<IVacancyRepository> repository,
        [Greedy] VacancyController sut,
        CancellationToken token)
    {
        // arrange
        entity.TransferInfo = null;
        entity.TrainingProvider_Address = null;
        entity.Qualifications = null;
        entity.ProviderReviewFieldIndicators = null;
        entity.EmployerLocations = null;
        entity.EmployerReviewFieldIndicators = null;
        entity.Skills = null;
        repository
            .Setup(x => x.GetOneLiveVacancyByVacancyReferenceAsync(vacancyReference, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // act
        var result = await sut.GetOneLiveVacancyByVacancyReference(repository.Object, vacancyReference, token);
        var payload = (result as Ok<Vacancy>)?.Value;

        // assert
        repository.Verify(x => x.GetOneLiveVacancyByVacancyReferenceAsync(vacancyReference, token), Times.Once());
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers().Excluding(c=>c.Skills).Excluding(c=>c.Qualifications));
    }

    [Test, MoqAutoData]
    public async Task Get_ReturnsNotFound_WhenEntity_NotFound(long vacancyReference,
        Mock<IVacancyRepository> repository,
        [Greedy] VacancyController sut,
        CancellationToken token)
    {
        // Arrange
        repository
            .Setup(x => x.GetOneLiveVacancyByVacancyReferenceAsync(vacancyReference, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VacancyEntity)null!);

        // Act
        var result = await sut.GetOneLiveVacancyByVacancyReference(repository.Object, vacancyReference, token);

        // Assert
        result.Should().BeOfType<NotFound>();
    }
}