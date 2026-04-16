using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
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
}