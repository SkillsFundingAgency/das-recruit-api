using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.Rules.VacancyRules;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Rules.VacancyRules;

public class WhenApplyingAnonymousVacancyRule
{
    [Test, MoqAutoData]
    public async Task Then_An_Anonymous_Vacancy_Fails(
        VacancySnapshot snapshot,
        VacancyAnonymousRule sut)
    {
        // arrange
        snapshot.EmployerNameOption = EmployerNameOption.Anonymous;

        // act
        var result = await sut.EvaluateAsync(snapshot, CancellationToken.None);

        // assert
        result.Score.Should().Be(100);
    }
    
    [Test, MoqAutoData]
    public async Task Then_An_Unanonymous_Vacancy_Passes(
        VacancySnapshot snapshot,
        VacancyAnonymousRule sut)
    {
        // arrange
        snapshot.EmployerNameOption = EmployerNameOption.TradingName;

        // act
        var result = await sut.EvaluateAsync(snapshot, CancellationToken.None);

        // assert
        result.Score.Should().Be(0);
    }
}