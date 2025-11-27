namespace SFA.DAS.Recruit.Api.UnitTests;

public static class RecruitFixtureBuilder
{
    public static IFixture FixtureFactory()
    {
        var fixture = FixtureBuilder.RecursiveMoqFixtureFactory();
        fixture.Customizations.Add(new VacancyReferenceSpecimenBuilder());
        fixture.Customizations.Add(new VacancyReviewEntitySpecimenBuilder());
        fixture.Customizations.Add(new VacancyEntitySpecimenBuilder());
        fixture.Customizations.Add(new RecruitNotificationEntitySpecimenBuilder());
        fixture.Customizations.Add(new UserEntitySpecimenBuilder());
        return fixture;
    }
}

public class RecruitAutoDataAttribute() : AutoDataAttribute(RecruitFixtureBuilder.FixtureFactory); 
public class RecruitInlineAutoDataAttribute(params object[] arguments) : InlineAutoDataAttribute(RecruitFixtureBuilder.FixtureFactory, arguments); 
