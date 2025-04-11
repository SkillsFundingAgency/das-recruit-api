using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfileAddress;

namespace Recruit.Api.UnitTests.Controllers.EmployerProfileAddressControllerTests;

public class WhenPostingAnEmployerProfileAddress
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
    public async Task Then_The_Profile_Is_Created(
        long accountLegalEntityId,
        Mock<IEmployerProfileAddressRepository> repository,
        PostEmployerProfileAddressRequest request,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        var entity = _fixture.Create<EmployerProfileAddressEntity>();
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<EmployerProfileAddressEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, true));

        // act
        var result = await sut.PostOne(repository.Object, accountLegalEntityId, request, token);
        var createdResult = result as Created<EmployerProfileAddress>; 
        var payload = createdResult?.Value;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(ItIs.EquivalentTo(request.ToDomain(accountLegalEntityId)), token), Times.Once);
        createdResult.Should().NotBeNull();
        createdResult.Location.Should().BeEquivalentTo($"/api/employerprofiles/{entity.AccountLegalEntityId}/addresses/{entity.Id}");
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
}