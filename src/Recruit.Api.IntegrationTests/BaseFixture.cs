using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public abstract class BaseFixture
{
    protected MockedTestServer Server;
    protected HttpClient Client;
    protected IFixture Fixture;
    
    [SetUp]
    public virtual void Setup()
    {
        Server = new MockedTestServer();
        Client = Server.CreateClient();
        Fixture = new Fixture();
        Fixture.Customizations.Add(new VacancyReferenceSpecimenBuilder());
        Fixture.Customizations.Add(new VacancyReviewEntitySpecimenBuilder());
        Fixture.Customizations.Add(new VacancyEntitySpecimenBuilder());
        Fixture.Customizations.Add(new ReportEntitySpecimenBuilder());
        Fixture.Customizations.Add(new VacancyAnalyticsEntitySpecimenBuilder());
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [TearDown]
    public virtual void Teardown()
    {
        Client.Dispose();
        Server.Dispose();
    }
}