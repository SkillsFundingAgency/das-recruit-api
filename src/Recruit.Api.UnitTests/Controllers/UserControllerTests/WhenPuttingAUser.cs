using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.User;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;
using SFA.DAS.Recruit.Api.Models.Requests.User;
using SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;
using SFA.DAS.Recruit.Api.Models.Responses.User;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.UserControllerTests;

internal class WhenPuttingAUser
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_User_Is_Updated(
        Guid userId,
        Mock<IUserRepository> repository,
        PutUserRequest request,
        UserEntity entity,
        [Greedy] UserController sut,
        CancellationToken token)
    {
        // arrange
        request.UserType = "Provider";
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<UserEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, false));

        // act
        var result = await sut.PutOne(repository.Object, userId, request, token);
        var payload = (result as Ok<PutUserResponse>)?.Value;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(ItIs.EquivalentTo(request.ToDomain(userId)), token), Times.Once);
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_User_Is_Created(
        Guid userId,
        Mock<IUserRepository> repository,
        PutUserRequest request,
        UserEntity entity,
        [Greedy] UserController sut,
        CancellationToken token)
    {
        // arrange
        request.UserType = "Employer";
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<UserEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, true));

        // act
        var result = await sut.PutOne(repository.Object, userId, request, token);
        var createdResult = result as Created<PutUserResponse>;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(ItIs.EquivalentTo(request.ToDomain(userId)), token), Times.Once);
        createdResult.Should().NotBeNull();
        createdResult.Value.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
        createdResult.Location.Should().Be($"/api/user/{entity.Id}");
    }
}