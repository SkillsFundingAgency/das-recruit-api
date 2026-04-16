using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.Rules.VacancyRules;
using ProhibitedContentType = SFA.DAS.Recruit.Api.Domain.Models.ProhibitedContentType;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Rules.VacancyRules;

public class WhenApplyingBannedPhraseRule
{
    public static readonly List<Action<VacancySnapshot, string>> FailingTestCases =
    [
        // props
        (x, v) => x.AdditionalQuestion1 = v,
        (x, v) => x.AdditionalQuestion2 = v,
        (x, v) => x.AdditionalTrainingDescription = v,
        (x, v) => x.ApplicationInstructions = v,
        (x, v) => x.Description = v,
        (x, v) => x.EmployerDescription = v,
        (x, v) => x.EmployerLocationInformation = v,
        (x, v) =>
        {
            x.EmployerNameOption = EmployerNameOption.Anonymous;
            x.EmployerName = v;
        },
        (x, v) =>
        {
            x.EmployerNameOption = EmployerNameOption.TradingName;
            x.EmployerName = v;
        },
        (x, v) => x.OutcomeDescription = v,
        (x, v) => x.ShortDescription = v,
        (x, v) => x.ThingsToConsider = v,
        (x, v) => x.Title = v,
        (x, v) => x.TrainingDescription = v,
        // deep objects
        (x, v) => x.EmployerContact.Name = v,
        (x, v) => x.ProviderContact.Name = v,
        (x, v) => x.Wage.WorkingWeekDescription = v,
        (x, v) => x.Wage.WageAdditionalInformation = v,
        (x, v) => x.EmployerLocations[0].AddressLine1 = v,
        (x, v) => x.EmployerLocations[0].AddressLine2 = v,
        (x, v) => x.EmployerLocations[0].AddressLine3 = v,
        (x, v) => x.EmployerLocations[0].AddressLine4 = v,
        (x, v) => x.EmployerLocations[0].Postcode = v,
        (x, v) => x.Skills[0] = v,
        (x, v) => x.Qualifications[0].Grade = v,
        (x, v) => x.Qualifications[0].Subject = v,
    ];
    
    [TestCaseSource(nameof(FailingTestCases))]
    public async Task Then_When_A_Tested_Field_Contains_A_Banned_Phrase_It_Fails(Action<VacancySnapshot, string> setter)
    {
        // arrange
        var fixture = new Fixture();
        var snapshot = fixture.Create<VacancySnapshot>();

        var repository = new Mock<IProhibitedContentRepository>(); 
        var sut = new VacancyBannedPhraseRule(repository.Object);
        
        repository
            .Setup(x => x.GetByContentTypeAsync(ProhibitedContentType.BannedPhrases, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ProhibitedContentEntity()
            {
                Content = "A banned phrase",
                ContentType = ProhibitedContentType.BannedPhrases
            }]);

        setter(snapshot, "Some content with a banned phrase(TM) in.");

        // act
        var result = await sut.EvaluateAsync(snapshot, CancellationToken.None);

        // assert
        result.Score.Should().Be(100);
    }
    
    [Test, MoqAutoData]
    public async Task Then_A_Snapshot_With_No_Offending_Fields_Passes(
        VacancySnapshot snapshot,
        [Frozen] Mock<IProhibitedContentRepository> repository,
        [Greedy] VacancyBannedPhraseRule sut)
    {
        // arrange
        repository
            .Setup(x => x.GetByContentTypeAsync(ProhibitedContentType.BannedPhrases, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ProhibitedContentEntity()
            {
                Content = "A banned phrase",
                ContentType = ProhibitedContentType.BannedPhrases
            }]);

        // act
        var result = await sut.EvaluateAsync(snapshot, CancellationToken.None);

        // assert
        result.Score.Should().Be(0);
    }
}