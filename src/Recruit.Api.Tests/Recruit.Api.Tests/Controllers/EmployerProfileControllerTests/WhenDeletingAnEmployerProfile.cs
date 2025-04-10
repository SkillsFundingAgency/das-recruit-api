using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Controllers.EmployerProfileControllerTests;

public class WhenDeletingAnEmployerProfile
{
    [Test, MoqAutoData]
    public async Task Then_The_Profile_Is_NotFound(
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.DeleteOneAsync(accountLegalEntityId, token))
            .ReturnsAsync(false);

        // act
        var result = await sut.DeleteOne(repository.Object, accountLegalEntityId, token);
        
        // assert
        result.Should().BeOfType<NotFound>();
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Profile_Is_Deleted(
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.DeleteOneAsync(accountLegalEntityId, token))
            .ReturnsAsync(true);

        // act
        var result = await sut.DeleteOne(repository.Object, accountLegalEntityId, token);
        
        // assert
        result.Should().BeOfType<NoContent>();
    }
}