using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Responses.Notifications;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.NotificationControllerTests;
[TestFixture]
internal class WhenGettingBatchByUserStatus
{
    [Test, RecruitAutoData]
    public async Task Then_Gets_Users_By_InActive_Status(
        Mock<INotificationsRepository> repository,
        Mock<IEmailFactory> emailFactory,
        List<RecruitNotificationEntity> entities,
        [Greedy] NotificationController sut,
        CancellationToken token)
    {
        //Arrange
        repository
            .Setup(x => x.GetBatchByUserInactiveStatusAsync(token))
            .ReturnsAsync(entities);

        //Act
        var result = await sut.GetBatchByUserStatus(UserStatus.Inactive, repository.Object, emailFactory.Object, token);
        result.Should().BeOfType<Ok<GetBatchByUserStatusResponse>>();
        var okResult = result as Ok<GetBatchByUserStatusResponse>;
        okResult.Should().NotBeNull();
        okResult.Value!.Emails.Count().Should().Be(entities.Count);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_Gets_Users_By_Active_Status(
        Mock<INotificationsRepository> repository,
        Mock<IEmailFactory> emailFactory,
        List<RecruitNotificationEntity> entities,
        [Greedy] NotificationController sut,
        CancellationToken token)
    {
        //Act
        var result = await sut.GetBatchByUserStatus(UserStatus.Active, repository.Object, emailFactory.Object, token);
        result.Should().BeOfType<BadRequest<string>>($"Only '{UserStatus.Inactive}' status is supported.");
    }
}
