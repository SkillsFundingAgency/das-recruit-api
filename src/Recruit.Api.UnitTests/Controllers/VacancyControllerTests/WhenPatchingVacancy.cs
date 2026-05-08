using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyControllerTests;

public class WhenPatchingVacancy
{
    [Test, RecruitAutoData]
    public async Task Then_A_Vacancy_State_Change_Is_Handled(
        VacancyEntity vacancyEntity,
        VacancyEntity updatedEntity,
        Mock<IVacancyRepository> repository,
        Mock<IEventsService> eventsService,
        [Greedy] VacancyController sut,
        CancellationToken token)
    {
        // arrange
        vacancyEntity.Status = VacancyStatus.Live;
        var patchDocument = new JsonPatchDocument<Vacancy>();
        patchDocument.Replace(x => x.Status, VacancyStatus.Closed);
        repository
            .Setup(x => x.GetOneAsync(vacancyEntity.Id, token))
            .ReturnsAsync(vacancyEntity);

        updatedEntity.Status = VacancyStatus.Closed;
        var upsertResult = UpsertResult.Create(updatedEntity, false, true);
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyEntity>(), token))
            .ReturnsAsync(upsertResult);

        // act
        var result = await sut.PatchOne(repository.Object, eventsService.Object, vacancyEntity.Id, patchDocument, token);

        // assert
        result.Should().BeOfType<Ok<Vacancy>>();
        eventsService.Verify(x => x.HandleVacancyStatusChange(upsertResult), Times.Once);
    }
}