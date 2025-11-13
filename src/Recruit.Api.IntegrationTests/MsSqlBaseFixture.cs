using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public abstract class MsSqlBaseFixture
{
    // private
    private MsSqlTestServer _server;
    private IServiceScope _serviceScope;
    private RecruitDataContext _dataContext;

    // protected
    protected IFixture Fixture;
    protected HttpClient Client;
    internal TestDataManager TestData { get; private set; }

    [SetUp]
    public virtual void Setup()
    {
        _server = new MsSqlTestServer();
        Client = _server.CreateClient();
        Fixture = new Fixture();
        Fixture.Customizations.Add(new VacancyReferenceSpecimenBuilder());
        Fixture.Customizations.Add(new VacancyReviewEntitySpecimenBuilder());
        Fixture.Customizations.Add(new VacancyEntitySpecimenBuilder());
        Fixture.Customizations.Add(new UserEntitySpecimenBuilder());
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _serviceScope = _server.Services.CreateScope();
        _dataContext = _serviceScope.ServiceProvider.GetRequiredService<RecruitDataContext>();
        
        var configuration = _server.Server.Services.GetService(typeof(IConfiguration)) as IConfiguration;
        TestData = new TestDataManager(_dataContext, Fixture, configuration.GetDbSchemaName());
    }

    [TearDown]
    public virtual void Teardown()
    {
        TestData = null!;
        _dataContext.Dispose();
        _serviceScope.Dispose();
        
        Client.Dispose();
        _server.Dispose();
    }
}