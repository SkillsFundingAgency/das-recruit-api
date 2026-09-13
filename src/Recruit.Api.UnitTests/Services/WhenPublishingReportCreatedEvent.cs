using NServiceBus;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Services;

public class WhenPublishingReportCreatedEvent
{
    [Test, MoqAutoData]
    public async Task Then_ReportCreatedEvent_Is_Published(
        ReportEntity entity,
        [Frozen] Mock<IMessageSession> messageSession,
        [Greedy] EventsService sut)
    {
        await sut.PublishReportCreatedEvent(entity);

        messageSession.Verify(x => x.Publish(
            It.Is<ReportCreatedEvent>(e =>
                e.ReportId == entity.Id &&
                e.UserId == entity.UserId &&
                e.OwnerType == entity.OwnerType),
            It.IsAny<PublishOptions>()), Times.Once);
    }
}
