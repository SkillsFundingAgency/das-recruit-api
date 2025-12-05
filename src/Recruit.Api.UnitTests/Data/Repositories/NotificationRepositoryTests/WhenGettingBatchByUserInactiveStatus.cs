using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.NotificationRepositoryTests;
[TestFixture]
internal class WhenGettingBatchByUserInactiveStatus
{
    [Test, RecursiveMoqAutoData]
    public async Task GetBatchByUserInactiveStatusAsync_ShouldReturnNotifications_ForUsersInactiveOverOneYear(
        List<RecruitNotificationEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] NotificationsRepository sut,
        CancellationToken token)
    {
        // Arrange
        var cutoff = DateTime.UtcNow.AddYears(-2);
        foreach (RecruitNotificationEntity recruitNotification in entities)
        {
            recruitNotification.User.LastSignedInDate = cutoff;
        }
        context.Setup(x => x.RecruitNotifications).ReturnsDbSet(entities);

        // Act
        var result = await sut.GetBatchByUserInactiveStatusAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.All(v => v.User.LastSignedInDate < DateTime.UtcNow).Should().BeTrue();
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetBatchByUserInactiveStatusAsync_ShouldReturnEmpty_WhenNoInactiveUsersExist(
        List<RecruitNotificationEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] NotificationsRepository sut,
        CancellationToken token)
    {
        // Arrange
        var cutoff = DateTime.UtcNow;
        foreach (RecruitNotificationEntity recruitNotification in entities)
        {
            recruitNotification.User.LastSignedInDate = cutoff;
        }
        context.Setup(x => x.RecruitNotifications).ReturnsDbSet(entities);

        // Act
        var result = await sut.GetBatchByUserInactiveStatusAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetBatchByUserInactiveStatusAsync_ShouldReturnOnlyDistinctUsers(
        List<RecruitNotificationEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] NotificationsRepository sut,
        CancellationToken token)
    {
        // Arrange
        var sameUser = entities[0].User;
        sameUser.LastSignedInDate = DateTime.UtcNow.AddYears(-2);
        foreach (RecruitNotificationEntity recruitNotification in entities)
        {
            recruitNotification.User = sameUser;
        }
        context.Setup(x => x.RecruitNotifications).ReturnsDbSet(entities);

        // Act
        var result = await sut.GetBatchByUserInactiveStatusAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.All(v => v.User.LastSignedInDate < DateTime.UtcNow).Should().BeTrue();
    }
}
