using FluentValidation;
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
    public async Task Then_The_Vacancy_Closed_Event_Is_Raised_When_The_Record_Is_Set_To_Closed(
        VacancyEntity vacancyEntity,
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
        
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(vacancyEntity, false));
        
        VacancyEntity? capturedEntity = null;
        eventsService
            .Setup(x => x.PublishVacancyClosedEvent(It.IsAny<VacancyEntity>()))
            .Callback<VacancyEntity>(x => capturedEntity = x)
            .Returns(Task.CompletedTask);

        // act
        var result = await sut.PatchOne(repository.Object, eventsService.Object, vacancyEntity.Id, patchDocument, token);

        // assert
        result.Should().BeOfType<Ok<Vacancy>>();
        eventsService.Verify(x => x.PublishVacancyClosedEvent(It.IsAny<VacancyEntity>()), Times.Once);
        capturedEntity.Should().NotBeNull();
        capturedEntity.Id.Should().Be(vacancyEntity.Id);
        capturedEntity.VacancyReference.Should().Be(vacancyEntity.VacancyReference);
    }
}