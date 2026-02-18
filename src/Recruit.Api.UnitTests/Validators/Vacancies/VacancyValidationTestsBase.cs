using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq.Protected;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;
using ProhibitedContentType = SFA.DAS.Recruit.Api.Domain.Models.ProhibitedContentType;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public abstract class VacancyValidationTestsBase
{
    private readonly Mock<IProhibitedContentRepository> _prohibitedContentRepository = new();
    private readonly IExternalWebsiteHealthCheckService _externalWebsiteHealthCheckService;
    private readonly IHtmlSanitizerService _htmlSanitizerService = new HtmlSanitizerService();
    protected TimeProvider TimeProvider = new FakeTimeProvider(new DateTimeOffset(DateTime.UtcNow));
    protected readonly Mock<IMinimumWageProvider> MinimumWageProvider;

    protected VacancyValidationTestsBase()
    {
        var externalWebsiteMessageHandler = new Mock<HttpMessageHandler>();
        externalWebsiteMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage _, CancellationToken _) => new HttpResponseMessage(HttpStatusCode.OK));
        _externalWebsiteHealthCheckService = new ExternalWebsiteHealthCheckService(Mock.Of<ILogger<ExternalWebsiteHealthCheckService>>(), new HttpClient(externalWebsiteMessageHandler.Object));
        _prohibitedContentRepository
            .Setup(x => x.GetByContentTypeAsync(ProhibitedContentType.Profanity,
                It.IsAny<CancellationToken>())).ReturnsAsync([
                new ProhibitedContentEntity {
                    Content = "bother",
                    ContentType = ProhibitedContentType.Profanity
                },
                new ProhibitedContentEntity {
                    Content = "balderdash",
                    ContentType = ProhibitedContentType.Profanity
                },
                new ProhibitedContentEntity {
                    Content = "dang",
                    ContentType = ProhibitedContentType.Profanity
                },
                new ProhibitedContentEntity {
                    Content = "drat",
                    ContentType = ProhibitedContentType.Profanity
                }
            ]);
        MinimumWageProvider = new Mock<IMinimumWageProvider>();
    }

    protected IEntityValidator<Vacancy, VacancyRuleSet> Validator
    {
        get
        {
            var fluentValidator = new VacancyValidator(_prohibitedContentRepository.Object, _htmlSanitizerService, TimeProvider, _externalWebsiteHealthCheckService, MinimumWageProvider.Object);
            return new EntityValidator<Vacancy, VacancyRuleSet>(fluentValidator);
        }
    }
}