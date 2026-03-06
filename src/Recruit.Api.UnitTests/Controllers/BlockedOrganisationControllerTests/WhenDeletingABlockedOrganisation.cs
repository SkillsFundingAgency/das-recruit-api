using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.BlockedOrganisationControllerTests;

public class WhenDeletingABlockedOrganisation
{
    [Test, MoqAutoData]
    public async Task Then_The_Organisation_Is_NotFound(
        Guid id,
        Mock<IBlockedOrganisationRepository> repository,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.DeleteOneAsync(id, token))
            .ReturnsAsync(false);

        // act
        var result = await sut.DeleteOne(repository.Object, id, token);
        
        // assert
        result.Should().BeOfType<NotFound>();
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Organisation_Is_Deleted(
        Guid id,
        Mock<IBlockedOrganisationRepository> repository,
        [Greedy] BlockedOrganisationController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.DeleteOneAsync(id, token))
            .ReturnsAsync(true);

        // act
        var result = await sut.DeleteOne(repository.Object, id, token);
        
        // assert
        result.Should().BeOfType<NoContent>();
    }
}
