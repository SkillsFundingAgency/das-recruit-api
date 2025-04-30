namespace SFA.DAS.Recruit.Api.IntegrationTests;

public abstract class BaseFixture
{
    protected TestServer Server;
    protected HttpClient Client;
    
    [SetUp]
    public virtual void Setup()
    {
        Server = new TestServer();
        Client = Server.CreateClient();
    }

    [TearDown]
    public virtual void Teardown()
    {
        Client.Dispose();
        Server.Dispose();
    }
}