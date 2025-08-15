using System.Net;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.User;
using SFA.DAS.Recruit.Api.UnitTests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.UserControllerTests;

public class WhenPatchingUser: BaseFixture
{
    [Test]
    public async Task Then_The_User_Is_NotFound()
    {
        // arrange
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>(10).ToList());

        var patchDocument = new JsonPatchDocument<RecruitUser>();
        patchDocument.Add(x => x.Email, "email");
        
        // act
        var response = await Client.PatchAsync($"{RouteNames.User}/{Guid.NewGuid()}", patchDocument);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [TestCase("Id")]
    [TestCase(nameof(PutUserRequest.CreatedDate))]
    [TestCase(nameof(PutUserRequest.UserType))]
    public async Task Patching_CreatedDate_Returns_BadRequest(string fieldName)
    {
        // arrange
        string path = $"/{fieldName}";
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);
    
        var patchDocument = new JsonPatchDocument<RecruitUser>();
        patchDocument.Operations.Add(new Operation<RecruitUser>("replace", path, "some value"));
        
        // act
        var response = await Client.PatchAsync($"{RouteNames.User}/{items[1].Id}", patchDocument);
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(1);
        errors.Errors.Should().ContainKey(path);
    }
    
    [Test]
    public async Task Then_The_User_Is_Patched()
    {
        // arrange - some gymnastics because the mock isn't stateful or a proper DbSet
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        var itemsClone = items.JsonClone();
        var targetItem = itemsClone[4].JsonClone();
        
        Server.DataContext
            .SetupSequence(x => x.UserEntities)
            .ReturnsDbSet(items)
            .ReturnsDbSet(itemsClone);

        Server.DataContext
            .SetupSequence(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([]);

        var patchDocument = new JsonPatchDocument<RecruitUser>();
        patchDocument.Replace(x => x.EmployerAccountIds, ["ABCD", "BCDE"]);
        
        // act
        var response = await Client.PatchAsync($"{RouteNames.User}/{targetItem.Id}", patchDocument);
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}