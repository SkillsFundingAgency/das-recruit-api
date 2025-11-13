using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.UserControllerTests;
[TestFixture]
internal class WhenGettingByStatus
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_Gets_Users_By_InActive_Status(
        Mock<IUserRepository> repository,
        List<UserEntity> entities,
        [Greedy] UserController sut,
        CancellationToken token)
    {
        //Arrange
        foreach (UserEntity userEntity in entities)
        {
            userEntity.LastSignedInDate = DateTime.UtcNow.AddYears(1);
        }
        repository
            .Setup(x => x.FindAllInActiveUsersAsync(token))
            .ReturnsAsync(entities);

        //Act
        var result = await sut.GetAllByStatus(repository.Object, UserStatus.Inactive, token);
        result.Should().BeOfType<Ok<List<RecruitUser>>>();
        var okResult = result as Ok<List<RecruitUser>>;
        okResult.Should().NotBeNull();
        okResult.Value!.Count.Should().Be(entities.Count);
        okResult.Value!.All(v => v.LastSignedInDate > DateTime.UtcNow.AddYears(-1)).Should().BeTrue();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_Gets_Users_By_Active_Status(
        Mock<IUserRepository> repository,
        List<UserEntity> entities,
        [Greedy] UserController sut,
        CancellationToken token)
    {
        //Act
        var result = await sut.GetAllByStatus(repository.Object, UserStatus.Active, token);
        result.Should().BeOfType<BadRequest<string>>($"Status 'Active' is not supported. Only 'inactive' is supported.");
    }
}