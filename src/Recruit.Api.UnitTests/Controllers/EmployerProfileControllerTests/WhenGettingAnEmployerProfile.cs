using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerProfileControllerTests;

public class WhenGettingAnEmployerProfile
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Profile_Is_Returned(
        EmployerProfileEntity entity,
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetOneAsync(accountLegalEntityId, token))
            .ReturnsAsync(entity);

        // act
        var result = await sut.GetOne(repository.Object, accountLegalEntityId, token);
        var payload = (result as Ok<EmployerProfile>)?.Value;
        
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
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Profiles_Are_Returned(
        List<EmployerProfileEntity> entities,
        long accountId,
        Mock<IEmployerProfileRepository> repository,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GetManyForAccountAsync(accountId, token))
            .ReturnsAsync(entities);

        // act
        var result = await sut.GetMany(repository.Object, accountId, token);
        var payload = (result as Ok<IEnumerable<EmployerProfile>>)?.Value;
        
        // assert
        payload.Should().BeEquivalentTo(entities, options => options.ExcludingMissingMembers());
    }
}