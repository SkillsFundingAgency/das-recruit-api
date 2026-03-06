using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Responses.BlockedOrganisation;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.BlockedOrganisationControllerTests;

public class WhenPatchingABlockedOrganisation
{   
    [Test, MoqAutoData]
    public async Task Then_The_Organisation_Is_NotFound(
        Guid id,
        Mock<IBlockedOrganisationRepository> repository,
        JsonPatchDocument patchRequest,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(id, token))
            .ReturnsAsync(() => null);

        // act
        var result = await sut.PatchOne(repository.Object, id, patchRequest, token);
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(It.IsAny<BlockedOrganisationEntity>(), It.IsAny<CancellationToken>()), Times.Never());
        result.Should().BeOfType<NotFound>();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Organisation_Is_Patched(
        BlockedOrganisationEntity entity,
        Guid id,
        string reason,
        string blockedStatus,
        Mock<IBlockedOrganisationRepository> repository,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        var patchRequest = new JsonPatchDocument();
        patchRequest.Operations.AddRange([
            new Operation("replace", "/Reason", null, reason),
            new Operation("replace", "/BlockedStatus", null, blockedStatus),
        ]);
        repository.Setup(x => x.GetOneAsync(id, token)).ReturnsAsync(() => entity);
        repository.Setup(x => x.UpsertOneAsync(It.IsAny<BlockedOrganisationEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(UpsertResult.Create(entity, false));

        // act
        var result = await sut.PatchOne(repository.Object, id, patchRequest, token);
        var patchResult = result as Ok<PatchBlockedOrganisationResponse>;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(entity, token), Times.Once());
        entity.Reason.Should().Be(reason);
        entity.BlockedStatus.Should().Be(blockedStatus);
        patchResult.Should().NotBeNull();
        patchResult.Value.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Invalid_Updates_Returns_BadRequest(
        BlockedOrganisationEntity entity,
        Guid id,
        Mock<IBlockedOrganisationRepository> repository,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        var patchRequest = new JsonPatchDocument();
        patchRequest.Operations.AddRange([new Operation("replace", "/BadPath", null, "A new value")]);
        repository.Setup(x => x.GetOneAsync(id, token)).ReturnsAsync(() => entity);

        // act
        var result = await sut.PatchOne(repository.Object, id, patchRequest, token);
        
        // assert
        result.Should().BeOfType<ValidationProblem>();
    }
}
