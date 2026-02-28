using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.BlockedOrganisationControllerTests;

public class WhenGettingBlockedOrganisations
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Organisation_Is_Returned(
        BlockedOrganisationEntity entity,
        Guid id,
        Mock<IBlockedOrganisationRepository> repository,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(id, token))
            .ReturnsAsync(entity);

        // act
        var result = await sut.GetOne(repository.Object, id, token);
        var payload = (result as Ok<BlockedOrganisation>)?.Value;
        
        // assert
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Organisation_Is_NotFound(
        Guid id,
        Mock<IBlockedOrganisationRepository> repository,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(id, token))
            .ReturnsAsync(() => null);

        // act
        var result = await sut.GetOne(repository.Object, id, token);
        
        // assert
        result.Should().BeOfType<NotFound>();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Organisations_Are_Returned(
        List<BlockedOrganisationEntity> entities,
        string organisationId,
        Mock<IBlockedOrganisationRepository> repository,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetByOrganisationIdAsync(organisationId, token))
            .ReturnsAsync(entities);

        // act
        var result = await sut.GetMany(repository.Object, organisationId, token);
        var payload = (result as Ok<IEnumerable<BlockedOrganisation>>)?.Value;
        
        // assert
        payload.Should().BeEquivalentTo(entities, options => options.ExcludingMissingMembers());
    }
}
