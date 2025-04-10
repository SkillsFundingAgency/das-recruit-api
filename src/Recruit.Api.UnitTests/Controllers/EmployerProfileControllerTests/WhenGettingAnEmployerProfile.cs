using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

namespace Recruit.Api.UnitTests.Controllers.EmployerProfileControllerTests;

public class WhenGettingAnEmployerProfile
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
    public async Task Then_The_Profile_Is_Returned(
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        var entity = _fixture.Create<EmployerProfileEntity>();
        repository
            .Setup(x => x.GetOneAsync(accountLegalEntityId, token))
            .ReturnsAsync(entity);

        // act
        var result = await sut.GetOne(repository.Object, accountLegalEntityId, token);
        var payload = (result as Ok<GetEmployerProfileResponse>)?.Value;
        
        // assert
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Profile_Is_NotFound(
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(accountLegalEntityId, token))
            .ReturnsAsync(() => null);

        // act
        var result = await sut.GetOne(repository.Object, accountLegalEntityId, token);
        
        // assert
        result.Should().BeOfType<NotFound>();
    }
}