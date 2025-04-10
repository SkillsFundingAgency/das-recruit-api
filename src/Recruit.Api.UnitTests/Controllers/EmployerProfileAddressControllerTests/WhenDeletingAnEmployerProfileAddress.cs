using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;

namespace Recruit.Api.UnitTests.Controllers.EmployerProfileAddressControllerTests;

public class WhenDeletingAnEmployerProfileAddress
{
    [Test, MoqAutoData]
    public async Task Then_The_Address_Is_NotFound(
        long accountLegalEntityId,
        int id,
        Mock<IEmployerProfileAddressRepository> repository,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.DeleteOneAsync(It.IsAny<EmployerProfileAddressKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // act
        var result = await sut.DeleteOne(repository.Object, accountLegalEntityId, id, token);
        
        // assert
        repository.Verify(x => x.DeleteOneAsync(ItIs.EquivalentTo(new EmployerProfileAddressKey(accountLegalEntityId, id)), token), Times.Once());
        result.Should().BeOfType<NotFound>();
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Address_Is_Deleted(
        long accountLegalEntityId,
        int id,
        Mock<IEmployerProfileAddressRepository> repository,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.DeleteOneAsync(It.IsAny<EmployerProfileAddressKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // act
        var result = await sut.DeleteOne(repository.Object, accountLegalEntityId, id, token);
        
        // assert
        repository.Verify(x => x.DeleteOneAsync(ItIs.EquivalentTo(new EmployerProfileAddressKey(accountLegalEntityId, id)), token), Times.Once());
        result.Should().BeOfType<NoContent>();
    }
}