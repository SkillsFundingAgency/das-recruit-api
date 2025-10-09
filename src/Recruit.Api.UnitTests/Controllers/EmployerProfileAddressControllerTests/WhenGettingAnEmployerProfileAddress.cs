using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerProfileAddressControllerTests;

public class WhenGettingAnEmployerProfileAddress
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Address_Is_Returned(
        EmployerProfileAddressEntity entity,
        long accountLegalEntityId,
        int id,
        Mock<IEmployerProfileAddressRepository> repository,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(It.IsAny<EmployerProfileAddressKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // act
        var result = await sut.GetOne(repository.Object, accountLegalEntityId, id, token);
        var payload = (result as Ok<EmployerProfileAddress>)?.Value;
        
        // assert
        repository.Verify(x => x.GetOneAsync(ItIs.EquivalentTo(new EmployerProfileAddressKey(accountLegalEntityId, id)), token), Times.Once());
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
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
            .Setup(x => x.GetOneAsync(It.IsAny<EmployerProfileAddressKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // act
        var result = await sut.GetOne(repository.Object, accountLegalEntityId, id, token);
        
        // assert
        repository.Verify(x => x.GetOneAsync(ItIs.EquivalentTo(new EmployerProfileAddressKey(accountLegalEntityId, id)), token), Times.Once());
        result.Should().BeOfType<NotFound>();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Addresses_Are_Returned(
        List<EmployerProfileAddressEntity> entities,
        long accountLegalEntityId,
        int id,
        Mock<IEmployerProfileAddressRepository> repository,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetManyAsync(accountLegalEntityId, token))
            .ReturnsAsync(entities);

        // act
        var result = await sut.GetMany(repository.Object, accountLegalEntityId, token);
        var payload = (result as Ok<IEnumerable<EmployerProfileAddress>>)?.Value;
        
        // assert
        payload.Should().BeEquivalentTo(entities, options => options.ExcludingMissingMembers());
    }
}