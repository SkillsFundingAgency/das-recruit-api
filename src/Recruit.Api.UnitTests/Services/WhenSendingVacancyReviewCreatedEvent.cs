using NServiceBus;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Services;

public class WhenSendingVacancyReviewCreatedEvent
{
    [Test, MoqAutoData]
    public async Task Then_The_Event_Is_Sent(
        Guid vacancyId,
        Guid vacancyReviewId,
        [Frozen] Mock<IMessageSession> messageSession,
        [Greedy] EventsService eventsService)
    {
        // arrange
        var entity = new VacancyReviewEntity
        {
            VacancyReference = 0,
            VacancyTitle = null,
            CreatedDate = default,
            SlaDeadLine = default,
            Status = ReviewStatus.New,
            SubmittedByUserEmail = null,
            ManualQaFieldIndicators = null,
            DismissedAutomatedQaOutcomeIndicators = null,
            UpdatedFieldIdentifiers = null,
            AccountId = 0,
            AccountLegalEntityId = 0,
            Ukprn = 0,
            OwnerType = OwnerType.Employer,
            
            // the two we're interested in
            Id = vacancyReviewId,
            VacancySnapshot = $"{{\"id\": \"{vacancyId}\"}}"
        };
        
        VacancyReviewCreatedEvent? capturedEvent = null;
        messageSession
            .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
            .Callback<object, PublishOptions>((x, _) => capturedEvent = x as VacancyReviewCreatedEvent)
            .Returns(Task.CompletedTask);

        // act
        await eventsService.PublishVacancyReviewCreatedEventAsync(entity);

        // assert
        capturedEvent.Should().NotBeNull();
        messageSession.Verify(x => x.Publish(capturedEvent, It.IsAny<PublishOptions>()), Times.Once);
        capturedEvent.VacancyId.Should().Be(vacancyId);
        capturedEvent.VacancyReviewId.Should().Be(vacancyReviewId);
        
    }
}