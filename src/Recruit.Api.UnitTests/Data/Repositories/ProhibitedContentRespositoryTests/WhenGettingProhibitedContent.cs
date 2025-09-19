using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ProhibitedContentRespositoryTests;

internal class WhenGettingProhibitedContent
{
    [Test]
    [MoqInlineAutoData(ProhibitedContentType.BannedPhrases)]
    [MoqInlineAutoData(ProhibitedContentType.Profanity)]
    public async Task Then_The_Correct_Items_Are_Returned(
        ProhibitedContentType prohibitedContentType,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ProhibitedContentRepository repository)
    {
        // arrange
        List<ProhibitedContentEntity> items = [
            new () { ContentType = ProhibitedContentType.BannedPhrases, Content = "BannedPhrase1"},
            new () { ContentType = ProhibitedContentType.BannedPhrases, Content = "BannedPhrase2"},
            new () { ContentType = ProhibitedContentType.BannedPhrases, Content = "BannedPhrase3"},
            new () { ContentType = ProhibitedContentType.Profanity, Content = "Profanity1"},
            new () { ContentType = ProhibitedContentType.Profanity, Content = "Profanity2"},
        ];
        var expectedItems = items.Where(x => x.ContentType == prohibitedContentType).ToList();
        context.Setup(x => x.ProhibitedContentEntities).ReturnsDbSet(items);
        
        // act
        var results = await repository.GetByContentTypeAsync(prohibitedContentType, CancellationToken.None);
        
        // assert
        results.Should().HaveCount(expectedItems.Count);
        results.Should().BeEquivalentTo(expectedItems);
    }
}