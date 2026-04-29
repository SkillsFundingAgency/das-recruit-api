using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyControllerTests;

public class WhenPuttingVacancy
{
    [Test, RecruitAutoData]
    public async Task Then_A_Vacancy_State_Change_Is_Handled(
        Guid id,
        PutVacancyRequest request,
        Mock<IVacancyRepository> repository,
        Mock<IEventsService> eventsService,
        Mock<IUserRepository> userRepository,
        Mock<IValidator<VacancyRequest>> validator,
        [Greedy] VacancyController sut,
        CancellationToken token)
    {
        // arrange
        request.Status = VacancyStatus.Closed;
        var entity = request.ToDomain(id);
        var upsertResult = UpsertResult.Create(entity, false, true);
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyEntity>(), token))
            .ReturnsAsync(upsertResult);

        // act
        var result = await sut.PutOne(repository.Object, userRepository.Object, eventsService.Object,validator.Object, id, request,null, false, token);

        // assert
        result.Should().BeOfType<Ok<SFA.DAS.Recruit.Api.Models.Vacancy>>();
        eventsService.Verify(x => x.HandleVacancyStatusChange(upsertResult), Times.Once);
    }
}