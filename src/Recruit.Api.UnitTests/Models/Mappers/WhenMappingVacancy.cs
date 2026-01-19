using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Models.Mappers;

public class WhenMappingVacancy
{
    [Test, RecruitAutoData]
    public void Then_Mapped_Entities_Should_Be_Identical(VacancyEntity entity)
    {
        // If this test fails then the VacancyMapper has not been updated
        // with new fields and will cause data loss when patching operations
        // happen.
        
        // arrange/act
        var vacancy = VacancyMapper.FromEntity(entity);
        var mappedEntity = VacancyMapper.ToEntity(vacancy);

        // assert
        mappedEntity.Should().BeEquivalentTo(entity);
    }
}