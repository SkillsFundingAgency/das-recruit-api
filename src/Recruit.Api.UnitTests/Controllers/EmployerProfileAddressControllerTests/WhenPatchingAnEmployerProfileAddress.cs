using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerProfileAddressControllerTests;

public class WhenPatchingAnEmployerProfileAddress
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
    public async Task Then_The_Profile_Is_NotFound(
        long accountLegalEntityId,
        int id,
        Mock<IEmployerProfileAddressRepository> repository,
        JsonPatchDocument patchRequest,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(It.IsAny<EmployerProfileAddressKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // act
        var result = await sut.PatchOne(repository.Object, accountLegalEntityId, id, patchRequest, token);
        
        // assert
        repository.Verify(x => x.GetOneAsync(ItIs.EquivalentTo(new EmployerProfileAddressKey(accountLegalEntityId, id)), token), Times.Once());
        repository.Verify(x => x.UpsertOneAsync(It.IsAny<EmployerProfileAddressEntity>(), It.IsAny<CancellationToken>()), Times.Never());
        result.Should().BeOfType<NotFound>();
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Profile_Is_Patched(
        long accountLegalEntityId,
        int id,
        string addressLine1,
        string postcode,
        Mock<IEmployerProfileAddressRepository> repository,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        var patchRequest = new JsonPatchDocument();
        patchRequest.Operations.AddRange([
            new Operation("replace", "/AddressLine1", null, addressLine1),
            new Operation("replace", "/Postcode", null, postcode),
        ]);

        var entity = _fixture.Create<EmployerProfileAddressEntity>();
        repository.Setup(x => x.GetOneAsync(It.IsAny<EmployerProfileAddressKey>(), It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        repository.Setup(x => x.UpsertOneAsync(It.IsAny<EmployerProfileAddressEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(UpsertResult.Create(entity, false));

        // act
        var result = await sut.PatchOne(repository.Object, accountLegalEntityId, id, patchRequest, token);
        var patchResult = result as Ok<EmployerProfileAddress>;
        
        // assert
        repository.Verify(x => x.GetOneAsync(ItIs.EquivalentTo(new EmployerProfileAddressKey(accountLegalEntityId, id)), token), Times.Once());
        repository.Verify(x => x.UpsertOneAsync(entity, token), Times.Once());
        entity.AddressLine1.Should().Be(addressLine1);
        entity.Postcode.Should().Be(postcode);
        patchResult.Should().NotBeNull();
        patchResult.Value.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
    [Test, MoqAutoData]
    public async Task Then_Invalid_Updates_Returns_BadRequest(
        long accountLegalEntityId,
        int id,
        Mock<IEmployerProfileAddressRepository> repository,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        var patchRequest = new JsonPatchDocument();
        patchRequest.Operations.AddRange([new Operation("replace", "/Foo", null, "A new value")]);

        var entity = _fixture.Create<EmployerProfileAddressEntity>();
        repository.Setup(x => x.GetOneAsync(It.IsAny<EmployerProfileAddressKey>(), It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        // act
        var result = await sut.PatchOne(repository.Object, accountLegalEntityId, id, patchRequest, token);
        
        // assert
        repository.Verify(x => x.GetOneAsync(ItIs.EquivalentTo(new EmployerProfileAddressKey(accountLegalEntityId, id)), token), Times.Once());
        result.Should().BeOfType<ValidationProblem>();
    }
    
    [Test, MoqAutoData]
    public async Task Then_Excluded_Field_Updates_Returns_BadRequest(
        long accountLegalEntityId,
        int id,
        Mock<IEmployerProfileAddressRepository> repository,
        [Greedy] EmployerProfileAddressController sut,
        CancellationToken token)
    {
        // arrange
        var patchRequest = new JsonPatchDocument();
        patchRequest.Operations.AddRange([new Operation("replace", $"/{nameof(EmployerProfileAddressEntity.AccountLegalEntityId)}", null, "A new value")]);

        var entity = _fixture.Create<EmployerProfileAddressEntity>();
        repository.Setup(x => x.GetOneAsync(It.IsAny<EmployerProfileAddressKey>(), It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        // act
        var result = await sut.PatchOne(repository.Object, accountLegalEntityId, id, patchRequest, token);
        
        // assert
        repository.Verify(x => x.GetOneAsync(ItIs.EquivalentTo(new EmployerProfileAddressKey(accountLegalEntityId, id)), token), Times.Once());
        result.Should().BeOfType<ValidationProblem>();
    }
}