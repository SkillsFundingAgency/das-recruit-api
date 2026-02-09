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

    protected VacancyValidationTestsBase()
    {
        _prohibitedContentRepository
            .Setup(x => x.GetByContentTypeAsync(ProhibitedContentType.Profanity,
                It.IsAny<CancellationToken>())).ReturnsAsync([
                new ProhibitedContentEntity {
                    Content = "bother",
                    ContentType = ProhibitedContentType.Profanity
                },

                new ProhibitedContentEntity() {
                    Content = "balderdash",
                    ContentType = ProhibitedContentType.Profanity
                },

                new ProhibitedContentEntity() {
                    Content = "dang",
                    ContentType = ProhibitedContentType.Profanity
                },

                new ProhibitedContentEntity() {
                    Content = "drat",
                    ContentType = ProhibitedContentType.Profanity
                }
            ]);
    }

    protected IEntityValidator<Vacancy, VacancyRuleSet> Validator
    {
        get
        {
            var fluentValidator = new VacancyValidator(_prohibitedContentRepository.Object);
            return new EntityValidator<Vacancy, VacancyRuleSet>(fluentValidator);
        }
    }
}