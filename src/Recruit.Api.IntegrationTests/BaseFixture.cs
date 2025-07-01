using SFA.DAS.Recruit.Api.UnitTests;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public abstract class BaseFixture
{
    protected TestServer Server;
    protected HttpClient Client;
    protected IFixture Fixture;
    
    [SetUp]
    public virtual void Setup()
    {
        Server = new TestServer();
        Client = Server.CreateClient();
        Fixture = new Fixture();
        Fixture.Customizations.Add(new VacancyReferenceSpecimenBuilder());
        Fixture.Customizations.Add(new VacancyReviewEntitySpecimenBuilder());
    }

    [TearDown]
    public virtual void Teardown()
    {
        Client.Dispose();
        Server.Dispose();
    }
}