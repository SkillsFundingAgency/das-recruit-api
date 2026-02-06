using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Validators;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public abstract class VacancyValidationTestsBase
{
    private readonly Mock<IProhibitedContentRepository> _prohibitedContentRepository = new();

    protected VacancyValidationTestsBase()
    {
        _prohibitedContentRepository
            .Setup(x => x.GetByContentTypeAsync(ProhibitedContentType.Profanity,
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProhibitedContentEntity> {
                new() {
                    Content = "bother",
                    ContentType = ProhibitedContentType.Profanity
                },
                new() {
                    Content = "balderdash",
                    ContentType = ProhibitedContentType.Profanity
                },
                new() {
                    Content = "dang",
                    ContentType = ProhibitedContentType.Profanity
                },
                new() {
                    Content = "drat",
                    ContentType = ProhibitedContentType.Profanity
                }
            });
    }

    protected IEntityValidator<VacancyEntity, VacancyEntityRuleSet> Validator
    {
        get
        {
            var fluentValidator = new VacancyEntityValidator(_prohibitedContentRepository.Object);
            return new EntityValidator<VacancyEntity, VacancyEntityRuleSet>(fluentValidator);
        }
    }
}