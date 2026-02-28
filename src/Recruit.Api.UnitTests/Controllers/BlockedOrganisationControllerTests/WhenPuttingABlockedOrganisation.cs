using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.BlockedOrganisation;
using SFA.DAS.Recruit.Api.Models.Responses.BlockedOrganisation;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.BlockedOrganisationControllerTests;

internal class WhenPuttingABlockedOrganisation
{
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Organisation_Is_Updated(
        Guid id,
        Mock<IBlockedOrganisationRepository> repository,
        PutBlockedOrganisationRequest request,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        var entity = _fixture.Create<BlockedOrganisationEntity>();
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<BlockedOrganisationEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, false));

        // act
        var result = await sut.PutOne(repository.Object, id, request, token);
        var payload = (result as Ok<PutBlockedOrganisationResponse>)?.Value;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(ItIs.EquivalentTo(request.ToDomain(id), options => options.Excluding(c=>c.UpdatedDate)), token), Times.Once);
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Organisation_Is_Created(
        Guid id,
        Mock<IBlockedOrganisationRepository> repository,
        PutBlockedOrganisationRequest request,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        var entity = _fixture.Create<BlockedOrganisationEntity>();
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<BlockedOrganisationEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, true));

        // act
        var result = await sut.PutOne(repository.Object, id, request, token);
        var createdResult = result as Created<PutBlockedOrganisationResponse>;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(ItIs.EquivalentTo(request.ToDomain(id), options => options.Excluding(c=>c.UpdatedDate)), token), Times.Once);
        createdResult.Should().NotBeNull();
        createdResult.Value.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
        createdResult.Location.Should().Be($"/api/blockedorganisations/{entity.Id}");
    }
}
