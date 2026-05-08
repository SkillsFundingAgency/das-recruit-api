using NServiceBus;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Services;

public class WhenSendingVacancyReviewApprovedEvent
{
    [Test]
    [MoqInlineAutoData(ManualQaOutcome.Approved)]
    [MoqInlineAutoData(ManualQaOutcome.Bypassed)]
    public async Task Then_The_Vacancy_Review_Approved_Event_Is_Published(
        ManualQaOutcome manualQaOutcome,
        Guid vacancyId,
        Guid vacancyReviewId,
        [Frozen] Mock<IMessageSession> messageSession,
        [Greedy] EventsService sut)
    {
        // arrange
        var entity = new VacancyReviewEntity
        {
            VacancyReference = 0,
            VacancyTitle = null!,
            CreatedDate = default,
            SlaDeadLine = default,
            SubmittedByUserEmail = null!,
            ManualQaFieldIndicators = null!,
            DismissedAutomatedQaOutcomeIndicators = null!,
            UpdatedFieldIdentifiers = null!,
            AccountId = 0,
            AccountLegalEntityId = 0,
            Ukprn = 0,
            OwnerType = OwnerType.Employer,
            
            // the ones we're interested in
            Id = vacancyReviewId,
            Status = ReviewStatus.Closed,
            VacancySnapshot = $"{{\"id\": \"{vacancyId}\"}}",
            ManualOutcome = manualQaOutcome.ToString(),
            SubmissionCount = 1
        };

        // act
        await sut.HandleVacancyReviewStatusChange(UpsertResult.Create(entity, false, true));

        // assert
        messageSession.Verify(x => x.Publish(It.Is<VacancyReviewApprovedEvent>(m => m.VacancyId == vacancyId && m.VacancyReviewId == vacancyReviewId), It.IsAny<PublishOptions>()), Times.Once);
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Vacancy_Review_Approved_Event_Is_Not_Published(
        Guid vacancyId,
        Guid vacancyReviewId,
        [Frozen] Mock<IMessageSession> messageSession,
        [Greedy] EventsService sut)
    {
        // arrange
        var entity = new VacancyReviewEntity
        {
            VacancyReference = 0,
            VacancyTitle = null!,
            CreatedDate = default,
            SlaDeadLine = default,
            SubmittedByUserEmail = null!,
            ManualQaFieldIndicators = null!,
            DismissedAutomatedQaOutcomeIndicators = null!,
            UpdatedFieldIdentifiers = null!,
            AccountId = 0,
            AccountLegalEntityId = 0,
            Ukprn = 0,
            OwnerType = OwnerType.Employer,
            
            // the ones we're interested in
            Id = vacancyReviewId,
            Status = ReviewStatus.Closed,
            VacancySnapshot = $"{{\"id\": \"{vacancyId}\"}}",
            ManualOutcome = nameof(ManualQaOutcome.Referred),
            SubmissionCount = 1
        };

        // act
        await sut.HandleVacancyReviewStatusChange(UpsertResult.Create(entity, false, true));

        // assert
        messageSession.Verify(x => x.Publish(It.IsAny<VacancyReviewApprovedEvent>(), It.IsAny<PublishOptions>()), Times.Never);
    }
}