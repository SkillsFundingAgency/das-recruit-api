using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.User;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.UserControllerTests;

internal class WhenGettingByEmail
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_User_Is_Retrieved(
        Mock<IUserRepository> repository,
        GetUserRequest request,
        UserEntity entity,
        [Greedy] UserController sut,
        CancellationToken token)
    {
        // arrange
        request.UserType = UserType.Provider;
        repository
            .Setup(x => x.FindUserByEmailAsync(request.Email, UserTypeExtensions.ToDomain(request.UserType), token))
            .ReturnsAsync(entity);

        // act
        var result = await sut.GetOneByEmail(repository.Object, request, token);
        var payload = (result as Ok<RecruitUser>)?.Value;

        // assert
        repository.Verify(x => x.FindUserByEmailAsync(request.Email, UserTypeExtensions.ToDomain(request.UserType), token), Times.Once);
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_User_Not_Found(
        Mock<IUserRepository> repository,
        GetUserRequest request,
        UserEntity entity,
        [Greedy] UserController sut,
        CancellationToken token)
    {
        // arrange
        request.UserType = UserType.Employer;
        repository
            .Setup(x => x.FindUserByEmailAsync(request.Email, UserTypeExtensions.ToDomain(request.UserType), token))
            .ReturnsAsync((UserEntity)null!);

        // act
        var result = await sut.GetOneByEmail(repository.Object, request, token);
        
        // assert
        result.Should().BeOfType<NotFound>();
    }
}