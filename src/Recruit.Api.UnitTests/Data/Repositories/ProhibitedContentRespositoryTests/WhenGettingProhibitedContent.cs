using Microsoft.Extensions.Caching.Memory;
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
    public async Task Then_The_Correct_Items_Are_Returned_and_If_Not_In_Cache_Then_Added_To_Cache(
        ProhibitedContentType prohibitedContentType,
        [Frozen] Mock<ICacheEntry> mockCacheEntry, 
        [Frozen] Mock<IRecruitDataContext> context,
        [Frozen] Mock<IMemoryCache> cache,
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
        object? outValue = null;
        cache.Setup(x => x.TryGetValue($"{nameof(ProhibitedContentRepository)}:{prohibitedContentType}", out outValue))
            .Returns(false);
        cache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(mockCacheEntry.Object);
        
        // act
        var results = await repository.GetByContentTypeAsync(prohibitedContentType, CancellationToken.None);
        
        // assert
        results.Should().BeEquivalentTo(expectedItems);
        cache.Verify(x => x.CreateEntry($"{nameof(ProhibitedContentRepository)}:{prohibitedContentType}"), Times.Once);
        mockCacheEntry.VerifySet(x => x.Value = expectedItems, Times.Once);
        mockCacheEntry.VerifySet(x => x.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1), Times.Once);
    }
    
    [Test]
    [MoqInlineAutoData(ProhibitedContentType.BannedPhrases)]
    [MoqInlineAutoData(ProhibitedContentType.Profanity)]
    public async Task Then_If_In_Cache_The_Items_Are_Returned_From_Cache(
        ProhibitedContentType prohibitedContentType,
        List<ProhibitedContentEntity>? items,
        [Frozen] Mock<IMemoryCache> cache,
        [Greedy] ProhibitedContentRepository repository)
    {
        // arrange
        cache.Setup(x => x.TryGetValue($"{nameof(ProhibitedContentRepository)}:{prohibitedContentType}", out items))
            .Returns(true);
        
        // act
        var results = await repository.GetByContentTypeAsync(prohibitedContentType, CancellationToken.None);
        
        // assert
        results.Should().BeEquivalentTo(items);
    }
}