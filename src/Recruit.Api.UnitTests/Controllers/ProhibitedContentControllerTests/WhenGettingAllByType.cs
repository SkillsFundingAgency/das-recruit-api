using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.ProhibitedContent;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using DomainProhibitedContentType = SFA.DAS.Recruit.Api.Domain.Models.ProhibitedContentType;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ProhibitedContentControllerTests;

internal class WhenGettingAllByType
{
    [Test]
    [MoqInlineAutoData(ProhibitedContentType.BannedPhrases)]
    [MoqInlineAutoData(ProhibitedContentType.Profanity)]
    public async Task Data_Is_Returned(
        ProhibitedContentType? prohibitedContentType,
        List<ProhibitedContentEntity> prohibitedContentEntities,
        Mock<IProhibitedContentRepository> repository, 
        [Greedy] ProhibitedContentController sut)
    {
        // arrange
        repository
            .Setup(x => x.GetByContentTypeAsync(It.IsAny<DomainProhibitedContentType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prohibitedContentEntities);
        var expectedValues = prohibitedContentEntities.Select(x => x.Content).ToList();
        
        // act
        var result = await sut.GetAllByType(repository.Object, prohibitedContentType, CancellationToken.None) as Ok<IEnumerable<string>>;
        
        // assert
        repository.Verify(x => x.GetByContentTypeAsync(prohibitedContentType.ToDomain(), CancellationToken.None), Times.Once);
        result.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expectedValues);
    }
}