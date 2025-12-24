using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.User;
using SFA.DAS.Recruit.Api.Models.Responses.User;
using SFA.DAS.Recruit.Api.Testing;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.UserControllerTests;

public class WhenPuttingUser: BaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.User}/{Guid.NewGuid()}", new { });
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(3);
        errors.Errors.Should().ContainKeys(
            nameof(PutUserRequest.Name),
            nameof(PutUserRequest.Email),
            nameof(PutUserRequest.UserType)
        );
    }
    
    [Test]
    public async Task Then_The_User_Is_Added()
    {
        // arrange
        var id = Guid.NewGuid();
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([]);
        
        var request = Fixture.Create<PutUserRequest>();
        var expectedEntity = request.ToDomain(id);
        
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.User}/{id}", request);
        var user = await response.Content.ReadAsAsync<PutUserResponse>();

        // assert
        user.Should().NotBeNull();
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.ToString().Should().Be($"/{RouteNames.User}/{user.Id}");

        Server.DataContext.Verify(x => x.UserEntities.Add(ItIs.EquivalentTo(expectedEntity)), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_Replaced()
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        var targetItem = items[5];
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);
        
        Server.DataContext
            .Setup(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([
                new UserEmployerAccountEntity { UserId = targetItem.Id, EmployerAccountId = 123, User = targetItem }
            ]);

        var request = Fixture.Create<PutUserRequest>();
        
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.User}/{targetItem.Id}", request);
        var user = await response.Content.ReadAsAsync<PutUserResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        user.Should().NotBeNull();
        Server.DataContext.Verify(x => x.SetValues(targetItem, It.IsAny<UserEntity>()), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        targetItem.EmployerAccounts.Should().HaveCount(3);
        targetItem.EmployerAccounts.Select(x => x.EmployerAccountId).Should().BeEquivalentTo(request.EmployerAccountIds);
    }
}