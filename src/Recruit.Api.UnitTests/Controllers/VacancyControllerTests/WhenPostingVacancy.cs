using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyControllerTests;

public class WhenPostingVacancy
{
    [Test, RecruitAutoData]
    public async Task Then_The_Vacancy_Closed_Event_Is_Raised_When_The_Record_Is_Set_To_Closed(
        Guid id,
        PostVacancyRequest request,
        Mock<IVacancyRepository> repository,
        Mock<IEventsService> eventsService,
        Mock<IUserRepository> userRepository,
        Mock<IValidator<VacancyRequest>> validator,
        [Greedy] VacancyController sut,
        CancellationToken token)
    {
        // arrange
        request.Status = VacancyStatus.Closed;
        request.Id = null;
        var entity = request.ToDomain();
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(entity, true));

        // act
        var result = await sut.PostOne(repository.Object, userRepository.Object, eventsService.Object, validator.Object, request, null, false, token);

        // assert
        result.Should().BeOfType<Created<SFA.DAS.Recruit.Api.Models.Vacancy>>();
        eventsService.Verify(x => x.PublishVacancyClosedEvent(entity), Times.Once);
    }

    [Test, RecruitAutoData]
    public async Task Then_Conflict_Is_Returned_When_Vacancy_With_Same_Id_Already_Exists(
        PostVacancyRequest request,
        VacancyEntity existingVacancy,
        Mock<IVacancyRepository> repository,
        Mock<IEventsService> eventsService,
        Mock<IUserRepository> userRepository,
        Mock<IValidator<VacancyRequest>> validator,
        [Greedy] VacancyController sut,
        CancellationToken token)
    {
        // arrange
        request.Id = existingVacancy.Id;
        repository
            .Setup(x => x.GetOneAsync(existingVacancy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingVacancy);

        // act
        var result = await sut.PostOne(repository.Object, userRepository.Object, eventsService.Object, validator.Object, request, null, false, token);

        // assert
        var conflictResult = result as IValueHttpResult;
        conflictResult.Should().NotBeNull();
        result.Should().BeAssignableTo<IStatusCodeHttpResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        var problemDetails = (conflictResult!.Value as ValidationProblemDetails)!;
        problemDetails.Errors.Should().ContainKey("Id");
        problemDetails.Errors["Id"].Should().Contain("Unable to create Vacancy. Vacancy already submitted");
    }

    [Test, RecruitAutoData]
    public async Task Then_Vacancy_Is_Created_When_Id_Supplied_But_No_Existing_Vacancy_Found(
        PostVacancyRequest request,
        Mock<IVacancyRepository> repository,
        Mock<IEventsService> eventsService,
        Mock<IUserRepository> userRepository,
        Mock<IValidator<VacancyRequest>> validator,
        [Greedy] VacancyController sut,
        CancellationToken token)
    {
        // arrange
        request.Id = Guid.NewGuid();
        request.Status = VacancyStatus.Submitted;
        var entity = request.ToDomain();
        repository
            .Setup(x => x.GetOneAsync(request.Id.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VacancyEntity)null!);
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(entity, true));

        // act
        var result = await sut.PostOne(repository.Object, userRepository.Object, eventsService.Object, validator.Object, request, null, false, token);

        // assert
        result.Should().BeOfType<Created<SFA.DAS.Recruit.Api.Models.Vacancy>>();
    }
}