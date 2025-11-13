using AutoFixture;
using AutoFixture.NUnit3;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Recruit.Api.Testing;

public static class RecruitFixtureBuilder
{
    public static IFixture FixtureFactory()
    {
        var fixture = FixtureBuilder.RecursiveMoqFixtureFactory();
        fixture.Customizations.Add(new VacancyReferenceSpecimenBuilder());
        fixture.Customizations.Add(new VacancyReviewEntitySpecimenBuilder());
        fixture.Customizations.Add(new VacancyEntitySpecimenBuilder());
        fixture.Customizations.Add(new RecruitNotificationEntitySpecimenBuilder());
        return fixture;
    }
}

public class RecruitAutoDataAttribute() : AutoDataAttribute(RecruitFixtureBuilder.FixtureFactory); 
public class RecruitInlineAutoDataAttribute(params object[] arguments) : InlineAutoDataAttribute(RecruitFixtureBuilder.FixtureFactory, arguments);