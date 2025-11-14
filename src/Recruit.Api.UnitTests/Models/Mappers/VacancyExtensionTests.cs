using System.Text.Json;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Models.Mappers;

public class VacancyExtensionTests
{
    [Test, RecursiveMoqAutoData]
    public void Then_The_Entity_Is_Mapped_To_Response_Dto(VacancyEntity entity)
    {
        entity.EmployerLocations = null;
        entity.EmployerReviewFieldIndicators = null;
        entity.ProviderReviewFieldIndicators = null;
        entity.TransferInfo = null;
        entity.Qualifications = "[]";
        entity.Skills = "[]";
        entity.Ukprn = null;
        
        var actual = entity.ToGetResponse();

        actual.Should().NotBeNull();
        actual.AccountId.Should().Be(entity.AccountId);
        actual.AccountLegalEntityId.Should().Be(entity.AccountLegalEntityId);
        actual.AdditionalQuestion1.Should().Be(entity.AdditionalQuestion1);
        actual.AdditionalQuestion2.Should().Be(entity.AdditionalQuestion2);
        actual.AdditionalTrainingDescription.Should().Be(entity.AdditionalTrainingDescription);
        actual.AnonymousReason.Should().Be(entity.AnonymousReason);
        actual.ApplicationInstructions.Should().Be(entity.ApplicationInstructions);
        actual.ApprenticeshipType.Should().Be(entity.ApprenticeshipType);
        actual.ApprovedDate.Should().Be(entity.ApprovedDate);
        actual.ClosedDate.Should().Be(entity.ClosedDate);
        actual.ClosingDate.Should().Be(entity.ClosingDate);
        actual.ClosureReason.Should().Be(entity.ClosureReason);
        actual.Contact.Should().BeEquivalentTo(new {
            Email = entity.ContactEmail,
            Phone = entity.ContactPhone,
            Name = entity.ContactName,
        });
        actual.CreatedDate.Should().Be(entity.CreatedDate);
        actual.DeletedDate.Should().Be(entity.DeletedDate);
        actual.Description.Should().Be(entity.Description);
        actual.DisabilityConfident.Should().Be(entity.DisabilityConfident);
        actual.EmployerDescription.Should().Be(entity.EmployerDescription);
        actual.EmployerLocationInformation.Should().Be(entity.EmployerLocationInformation);
        actual.EmployerLocationOption.Should().Be(entity.EmployerLocationOption);
        actual.EmployerNameOption.Should().Be(entity.EmployerNameOption);
        actual.EmployerRejectedReason.Should().Be(entity.EmployerRejectedReason);
        actual.EmployerReviewFieldIndicators.Should().BeNull();
        actual.EmployerWebsiteUrl.Should().Be(entity.EmployerWebsiteUrl);
        actual.GeoCodeMethod.Should().Be(entity.GeoCodeMethod);
        actual.HasChosenProviderContactDetails.Should().Be(entity.HasChosenProviderContactDetails);
        actual.HasOptedToAddQualifications.Should().Be(entity.HasOptedToAddQualifications);
        actual.HasSubmittedAdditionalQuestions.Should().Be(entity.HasSubmittedAdditionalQuestions);
        actual.Id.Should().Be(entity.Id);
        actual.LastUpdatedDate.Should().Be(entity.LastUpdatedDate);
        actual.LegalEntityName.Should().Be(entity.LegalEntityName);
        actual.LiveDate.Should().Be(entity.LiveDate);
        actual.NumberOfPositions.Should().Be(entity.NumberOfPositions);
        actual.OutcomeDescription.Should().Be(entity.OutcomeDescription);
        actual.OwnerType.Should().Be(entity.OwnerType);
        actual.ProgrammeId.Should().Be(entity.ProgrammeId);
        actual.ProviderReviewFieldIndicators.Should().BeNull();
        actual.Qualifications.Should().BeEmpty();
        actual.ReviewCount.Should().Be(entity.ReviewCount);
        actual.ReviewRequestedByUserId.Should().Be(entity.ReviewRequestedByUserId);
        actual.ReviewRequestedDate.Should().Be(entity.ReviewRequestedDate);
        actual.ShortDescription.Should().Be(entity.ShortDescription);
        actual.Skills.Should().BeEmpty();
        actual.SourceOrigin.Should().Be(entity.SourceOrigin);
        actual.SourceType.Should().Be(entity.SourceType);
        actual.SourceVacancyReference.Should().Be(entity.SourceVacancyReference);
        actual.StartDate.Should().Be(entity.StartDate);
        actual.Status.Should().Be(entity.Status);
        actual.SubmittedByUserId.Should().Be(entity.SubmittedByUserId);
        actual.SubmittedDate.Should().Be(entity.SubmittedDate);
        actual.ThingsToConsider.Should().Be(entity.ThingsToConsider);
        actual.Title.Should().Be(entity.Title);
        actual.TrainingDescription.Should().Be(entity.TrainingDescription);
        actual.TrainingProvider.Should().BeNull();
        actual.TransferInfo.Should().BeNull();
        actual.VacancyReference.Should().Be(entity.VacancyReference);
        actual.Wage.Should().BeEquivalentTo(new {
            CompanyBenefitsInformation = entity.Wage_CompanyBenefitsInformation,
            Duration = entity.Wage_Duration,
            DurationUnit = entity.Wage_DurationUnit,
            FixedWageYearlyAmount = entity.Wage_FixedWageYearlyAmount,
            WageAdditionalInformation = entity.Wage_WageAdditionalInformation,
            WageType = entity.Wage_WageType,
            WeeklyHours = entity.Wage_WeeklyHours,
            WorkingWeekDescription = entity.Wage_WorkingWeekDescription,
        });


    }

    [Test, RecursiveMoqInlineAutoData]
    public void Then_Null_For_Lists_Should_Be_Mapped_To_Empty(VacancyEntity entity)
    {
        entity.Skills = null;
        entity.Qualifications = null;
        entity.EmployerLocations = null;
        entity.EmployerReviewFieldIndicators = null;
        entity.ProviderReviewFieldIndicators = null;
        entity.TransferInfo = null;
        entity.Ukprn = null;
        
        var actual = entity.ToGetResponse();

        actual.Qualifications.Should().BeEmpty();
        actual.Skills.Should().BeEmpty();
    }
    
    [Test, RecursiveMoqInlineAutoData]
    public void Then_If_Null_For_EmployerLocationOption_And_One_Address_Sets_Location_Option_To_OneLocation(VacancyEntity entity, Address address)
    {
        entity.Skills = null;
        entity.Qualifications = null;
        entity.EmployerLocations = JsonSerializer.Serialize(new List<Address>{address}, JsonConfig.Options);
        entity.EmployerReviewFieldIndicators = null;
        entity.ProviderReviewFieldIndicators = null;
        entity.TransferInfo = null;
        entity.Ukprn = null;
        entity.EmployerLocationOption = null;
        
        
        var actual = entity.ToGetResponse();

        actual.EmployerLocationOption.Should().Be(AvailableWhere.OneLocation);
    }
    
    [Test, RecursiveMoqInlineAutoData]
    public void Then_If_Null_For_EmployerLocationOption_And_More_Than_One_Address_Sets_Location_Option_To_OneLocation(VacancyEntity entity, List<Address> addresses)
    {
        entity.Skills = null;
        entity.Qualifications = null;
        entity.EmployerLocations = JsonSerializer.Serialize(addresses, JsonConfig.Options);
        entity.EmployerReviewFieldIndicators = null;
        entity.ProviderReviewFieldIndicators = null;
        entity.TransferInfo = null;
        entity.Ukprn = null;
        entity.EmployerLocationOption = null;
        
        
        var actual = entity.ToGetResponse();

        actual.EmployerLocationOption.Should().Be(AvailableWhere.MultipleLocations);
    }
    
    [Test, RecursiveMoqInlineAutoData]
    public void Then_If_Null_For_EmployerLocationOption_And_No_Address_Sets_Location_Option_To_AcrossEngland(VacancyEntity entity)
    {
        entity.Skills = null;
        entity.Qualifications = null;
        entity.EmployerLocations = null;
        entity.EmployerReviewFieldIndicators = null;
        entity.ProviderReviewFieldIndicators = null;
        entity.TransferInfo = null;
        entity.Ukprn = null;
        entity.EmployerLocationOption = null;
        
        
        var actual = entity.ToGetResponse();

        actual.EmployerLocationOption.Should().Be(AvailableWhere.AcrossEngland);
    }
}