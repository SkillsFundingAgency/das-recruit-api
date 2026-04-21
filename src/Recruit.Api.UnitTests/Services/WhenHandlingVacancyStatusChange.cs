using Esfa.Recruit.Vacancies.Client.Domain.Events;
using NServiceBus;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Services;

public class WhenHandlingVacancyStatusChange
{
    [Test, MoqAutoData]
    public async Task Then_Null_Parameter_Is_Handled(
        VacancyEntity vacancyEntity,
        [Frozen] Mock<IMessageSession> messageSession,
        [Greedy] EventsService sut)
    {
        // act
        var action = async () => await sut.HandleVacancyStatusChange(null!);

        // assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_The_Status_Has_Not_Changed_No_Event_Will_Be_Sent(
        VacancyEntity vacancyEntity,
        [Frozen] Mock<IMessageSession> messageSession,
        [Greedy] EventsService sut)
    {
        // act
        await sut.HandleVacancyStatusChange(UpsertResult.Create(vacancyEntity, false, false));

        // assert
        messageSession.Verify(x => x.Publish(It.IsAny<It.IsAnyType>(), It.IsAny<PublishOptions>()), Times.Never);
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Vacancy_Closed_Event_Is_Sent(
        VacancyEntity vacancyEntity,
        [Frozen] Mock<IMessageSession> messageSession,
        [Greedy] EventsService sut)
    {
        // arrange
        vacancyEntity.Status = VacancyStatus.Closed;
        
        // act
        await sut.HandleVacancyStatusChange(UpsertResult.Create(vacancyEntity, false, true));

        // assert
        messageSession.Verify(x => x.Publish(
            It.Is<VacancyClosedEvent>(e => e.VacancyId == vacancyEntity.Id && e.VacancyReference == vacancyEntity.VacancyReference),
            It.IsAny<PublishOptions>()
            ), Times.Once);
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Vacancy_Live_Event_Is_Sent(
        VacancyEntity vacancyEntity,
        [Frozen] Mock<IMessageSession> messageSession,
        [Greedy] EventsService sut)
    {
        // arrange
        vacancyEntity.Status = VacancyStatus.Live;
        
        // act
        await sut.HandleVacancyStatusChange(UpsertResult.Create(vacancyEntity, false, true));

        // assert
        messageSession.Verify(x => x.Publish(
            It.Is<VacancyLiveEvent>(e => e.VacancyId == vacancyEntity.Id && e.VacancyReference == vacancyEntity.VacancyReference),
            It.IsAny<PublishOptions>()
        ), Times.Once);
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Vacancy_Approved_Event_Is_Sent(
        VacancyEntity vacancyEntity,
        [Frozen] Mock<IMessageSession> messageSession,
        [Frozen] Mock<IEncodingService> encodingService,
        [Greedy] EventsService sut)
    {
        // arrange
        vacancyEntity.Status = VacancyStatus.Approved;
        
        // act
        await sut.HandleVacancyStatusChange(UpsertResult.Create(vacancyEntity, false, true));

        // assert
        encodingService.Verify(x => x.Encode(vacancyEntity.AccountLegalEntityId!.Value, EncodingType.PublicAccountLegalEntityId), Times.Once);
        messageSession.Verify(x => x.Publish(
            It.Is<VacancyApprovedEvent>(e => e.VacancyId == vacancyEntity.Id && e.VacancyReference == vacancyEntity.VacancyReference),
            It.IsAny<PublishOptions>()
        ), Times.Once);
    }
}