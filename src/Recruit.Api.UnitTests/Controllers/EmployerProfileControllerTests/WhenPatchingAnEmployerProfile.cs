using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerProfileControllerTests;

public class WhenPatchingAnEmployerProfile
{   
    [Test, MoqAutoData]
    public async Task Then_The_Profile_Is_NotFound(
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        JsonPatchDocument patchRequest,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(accountLegalEntityId, token))
            .ReturnsAsync(() => null);

        // act
        var result = await sut.PatchOne(repository.Object, accountLegalEntityId, patchRequest, token);
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(It.IsAny<EmployerProfileEntity>(), It.IsAny<CancellationToken>()), Times.Never());
        result.Should().BeOfType<NotFound>();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Profile_Is_Patched(
        EmployerProfileEntity entity,
        long accountLegalEntityId,
        string aboutOrganisation,
        string tradingName,
        Mock<IEmployerProfileRepository> repository,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        var patchRequest = new JsonPatchDocument();
        patchRequest.Operations.AddRange([
            new Operation("replace", "/AboutOrganisation", null, aboutOrganisation),
            new Operation("replace", "/TradingName", null, tradingName),
        ]);
        repository.Setup(x => x.GetOneAsync(accountLegalEntityId, token)).ReturnsAsync(() => entity);
        repository.Setup(x => x.UpsertOneAsync(It.IsAny<EmployerProfileEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(UpsertResult.Create(entity, false));

        // act
        var result = await sut.PatchOne(repository.Object, accountLegalEntityId, patchRequest, token);
        var patchResult = result as Ok<PatchEmployerProfileResponse>;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(entity, token), Times.Once());
        entity.TradingName.Should().Be(tradingName);
        entity.AboutOrganisation.Should().Be(aboutOrganisation);
        patchResult.Should().NotBeNull();
        patchResult.Value.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Invalid_Updates_Returns_BadRequest(
        EmployerProfileEntity entity,
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        var patchRequest = new JsonPatchDocument();
        patchRequest.Operations.AddRange([new Operation("replace", "/Foo", null, "A new value")]);
        repository.Setup(x => x.GetOneAsync(accountLegalEntityId, token)).ReturnsAsync(() => entity);

        // act
        var result = await sut.PatchOne(repository.Object, accountLegalEntityId, patchRequest, token);
        
        // assert
        result.Should().BeOfType<ValidationProblem>();
    }
}