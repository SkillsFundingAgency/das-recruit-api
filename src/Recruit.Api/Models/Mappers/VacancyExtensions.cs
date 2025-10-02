using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class VacancyExtensions
{
    public static VacancyEntity ToDomain(this PostVacancyRequest request)
    {
        return new VacancyEntity {
            AccountId = request.AccountId,
            AccountLegalEntityId = request.AccountLegalEntityId,
            AdditionalQuestion1 = request.AdditionalQuestion1,
            AdditionalQuestion2 = request.AdditionalQuestion2,
            AdditionalTrainingDescription = request.AdditionalTrainingDescription,
            AnonymousReason = request.AnonymousReason,
            ApplicationInstructions = request.ApplicationInstructions,
            ApplicationMethod = request.ApplicationMethod,
            ApplicationUrl = request.ApplicationUrl,
            ApprenticeshipType = request.ApprenticeshipType,
            ApprovedDate = request.ApprovedDate,
            ClosedDate = request.ClosedDate,
            ClosingDate = request.ClosingDate,
            ClosureReason = request.ClosureReason,
            ContactEmail = request.Contact?.Email,
            ContactName = request.Contact?.Name,
            ContactPhone = request.Contact?.Phone,
            DeletedDate = request.DeletedDate,
            Description = request.Description,
            DisabilityConfident = request.DisabilityConfident,
            EmployerDescription = request.EmployerDescription,
            EmployerLocationInformation = request.EmployerLocationInformation,
            EmployerLocationOption = request.EmployerLocationOption,
            EmployerLocations = ApiUtils.SerializeOrNull(request.EmployerLocations),
            EmployerName = request.EmployerName,
            EmployerNameOption = request.EmployerNameOption,
            EmployerRejectedReason = request.EmployerRejectedReason,
            EmployerReviewFieldIndicators = ApiUtils.SerializeOrNull(request.EmployerReviewFieldIndicators),
            EmployerWebsiteUrl = request.EmployerWebsiteUrl,
            GeoCodeMethod = request.GeoCodeMethod,
            HasChosenProviderContactDetails = request.HasChosenProviderContactDetails,
            HasOptedToAddQualifications = request.HasOptedToAddQualifications,
            HasSubmittedAdditionalQuestions = request.HasSubmittedAdditionalQuestions,
            LastUpdatedDate = request.LastUpdatedDate,
            LegalEntityName = request.LegalEntityName,
            LiveDate = request.LiveDate,
            NumberOfPositions = request.NumberOfPositions,
            OutcomeDescription = request.OutcomeDescription,
            OwnerType = request.OwnerType,
            ProgrammeId = request.ProgrammeId,
            ProviderReviewFieldIndicators = ApiUtils.SerializeOrNull(request.ProviderReviewFieldIndicators),
            Qualifications = ApiUtils.SerializeOrNull(request.Qualifications),
            ReviewCount = request.ReviewCount,
            ReviewRequestedDate = request.ReviewRequestedDate,
            ShortDescription = request.ShortDescription,
            Skills = ApiUtils.SerializeOrNull(request.Skills),
            SourceOrigin = request.SourceOrigin,
            SourceType = request.SourceType,
            SourceVacancyReference = request.SourceVacancyReference,
            StartDate = request.StartDate,
            Status = request.Status ?? VacancyStatus.Draft,
            //SubmittedByUserId = request.SubmittedByUserId,
            SubmittedDate = request.SubmittedDate,
            ThingsToConsider = request.ThingsToConsider,
            Title = request.Title,
            TrainingDescription = request.TrainingDescription,
            Ukprn = (int?)request.TrainingProvider?.Ukprn,
            TrainingProvider_Address = ApiUtils.SerializeOrNull(request.TrainingProvider?.Address),
            TrainingProvider_Name = request.TrainingProvider?.Name,
            TransferInfo = ApiUtils.SerializeOrNull(request.TransferInfo),
            Wage_Duration = request.Wage?.Duration,
            Wage_CompanyBenefitsInformation = request.Wage?.CompanyBenefitsInformation,
            Wage_DurationUnit = request.Wage?.DurationUnit,
            Wage_FixedWageYearlyAmount = request.Wage?.FixedWageYearlyAmount,
            Wage_WageAdditionalInformation = request.Wage?.WageAdditionalInformation,
            Wage_WageType = request.Wage?.WageType,
            Wage_WeeklyHours = request.Wage?.WeeklyHours,
            Wage_WorkingWeekDescription = request.Wage?.WorkingWeekDescription,
        };
    }

    private static Vacancy ToResponseDto(this VacancyEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new Vacancy {
            AccountId = entity.AccountId,
            AccountLegalEntityId = entity.AccountLegalEntityId,
            AdditionalQuestion1 = entity.AdditionalQuestion1,
            AdditionalQuestion2 = entity.AdditionalQuestion2,
            AdditionalTrainingDescription = entity.AdditionalTrainingDescription,
            AnonymousReason = entity.AnonymousReason,
            ApplicationInstructions = entity.ApplicationInstructions,
            ApplicationMethod = entity.ApplicationMethod,
            ApplicationUrl = entity.ApplicationUrl,
            ApprenticeshipType = entity.ApprenticeshipType,
            ApprovedDate = entity.ApprovedDate,
            ClosedDate = entity.ClosedDate,
            ClosingDate = entity.ClosingDate,
            ClosureReason = entity.ClosureReason,
            Contact = entity.ContactEmail is null ? null : new ContactDetail()
            {
                Email = entity.ContactEmail,
                Name = entity.ContactName!,
                Phone = entity.ContactPhone!,
            },
            CreatedDate = entity.CreatedDate,
            DeletedDate = entity.DeletedDate,
            Description = entity.Description,
            DisabilityConfident = entity.DisabilityConfident,
            EmployerDescription = entity.EmployerDescription,
            EmployerLocationInformation = entity.EmployerLocationInformation,
            EmployerLocationOption = entity.EmployerLocationOption,
            EmployerLocations = ApiUtils.DeserializeOrNull<List<Address>>(entity.EmployerLocations),
            EmployerName = entity.EmployerName,
            EmployerNameOption = entity.EmployerNameOption,
            EmployerRejectedReason = entity.EmployerRejectedReason,
            EmployerReviewFieldIndicators = ApiUtils.DeserializeOrNull<List<ReviewFieldIndicator>>(entity.EmployerReviewFieldIndicators),
            EmployerWebsiteUrl = entity.EmployerWebsiteUrl,
            GeoCodeMethod = entity.GeoCodeMethod,
            HasChosenProviderContactDetails = entity.HasChosenProviderContactDetails,
            HasOptedToAddQualifications = entity.HasOptedToAddQualifications,
            HasSubmittedAdditionalQuestions = entity.HasSubmittedAdditionalQuestions,
            Id = entity.Id,
            LastUpdatedDate = entity.LastUpdatedDate,
            LegalEntityName = entity.LegalEntityName,
            LiveDate = entity.LiveDate,
            NumberOfPositions = entity.NumberOfPositions,
            OutcomeDescription = entity.OutcomeDescription,
            OwnerType = entity.OwnerType,
            ProgrammeId = entity.ProgrammeId,
            ProviderReviewFieldIndicators = ApiUtils.DeserializeOrNull<List<ReviewFieldIndicator>>(entity.ProviderReviewFieldIndicators),
            Qualifications = ApiUtils.DeserializeOrNull<List<Qualification>>(entity.Qualifications),
            ReviewCount = entity.ReviewCount,
            ReviewRequestedByUserId = entity.ReviewRequestedByUserId,
            ReviewRequestedDate = entity.ReviewRequestedDate,
            ShortDescription = entity.ShortDescription,
            Skills = ApiUtils.DeserializeOrNull<List<string>>(entity.Skills),
            SourceOrigin = entity.SourceOrigin,
            SourceType = entity.SourceType,
            SourceVacancyReference = entity.SourceVacancyReference,
            StartDate = entity.StartDate,
            Status = entity.Status,
            SubmittedByUserId = entity.SubmittedByUserId,
            SubmittedDate = entity.SubmittedDate,
            ThingsToConsider = entity.ThingsToConsider,
            Title = entity.Title,
            TrainingDescription = entity.TrainingDescription,
            TrainingProvider = entity.Ukprn is null ? null :  new TrainingProvider
            {
                Ukprn = entity.Ukprn,
                Name = entity.TrainingProvider_Name!,
                Address = ApiUtils.DeserializeOrNull<TrainingProviderAddress>(entity.TrainingProvider_Address)!,
            },
            TransferInfo = ApiUtils.DeserializeOrNull<TransferInfo>(entity.TransferInfo),
            VacancyReference = entity.VacancyReference,
            Wage = entity.Wage_WageType is null ? null : new Wage
            {
                CompanyBenefitsInformation = entity.Wage_CompanyBenefitsInformation,
                Duration = entity.Wage_Duration,
                DurationUnit = entity.Wage_DurationUnit,
                FixedWageYearlyAmount = entity.Wage_FixedWageYearlyAmount,
                WageAdditionalInformation = entity.Wage_WageAdditionalInformation,
                WageType = entity.Wage_WageType,
                WeeklyHours = entity.Wage_WeeklyHours,
                WorkingWeekDescription = entity.Wage_WorkingWeekDescription,
            },
        };
    }

    private static VacancySummary ToSummaryDto(this VacancyEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var transferInfo = ApiUtils.DeserializeOrNull<TransferInfo>(entity.TransferInfo);
        return new VacancySummary {
            Id = entity.Id,
            VacancyReference = entity.VacancyReference,
            Title = entity.Title,
            EmployerName = entity.EmployerName,
            ClosingDate = entity.ClosingDate,
            Status = entity.Status,
            AccountLegalEntityId = entity.AccountLegalEntityId?.ToString(),
            EmployerAccountId = entity.AccountId.ToString(),
            CreatedDate = entity.CreatedDate,
            Duration = entity.Wage_Duration,
            DurationUnit = entity.Wage_DurationUnit,
            ApplicationMethod = entity.ApplicationMethod,
            ProgrammeId = entity.ProgrammeId,
            StartDate = entity.StartDate,
            LegalEntityName = entity.LegalEntityName,
            TransferInfoUkprn = transferInfo?.Ukprn,
            TransferInfoProviderName = transferInfo?.ProviderName,
            TransferInfoReason = transferInfo?.Reason,
            TransferInfoTransferredDate = transferInfo?.TransferredDate,
            Ukprn = entity.Ukprn,
            TrainingProviderName = entity.TrainingProvider_Name,
            ApprenticeshipType = entity.ApprenticeshipType,
            IsTraineeship = false,
            IsTaskListCompleted = entity.OwnerType is OwnerType.Employer or OwnerType.Provider && entity.HasSubmittedAdditionalQuestions is true,
            HasChosenProviderContactDetails = entity.HasChosenProviderContactDetails,
            ClosedDate = entity.ClosedDate,
            ClosureReason = entity.ClosureReason,
        };
    }


    public static Vacancy ToGetResponse(this VacancyEntity entity)
    {
        return ToResponseDto(entity);
    }

    public static Vacancy ToPostResponse(this VacancyEntity entity)
    {
        return ToResponseDto(entity);
    }
    
    public static Vacancy ToPatchResponse(this VacancyEntity entity)
    {
        return ToResponseDto(entity);
    }
    
    public static Vacancy ToPutResponse(this VacancyEntity entity)
    {
        return ToResponseDto(entity);
    }

    public static VacancyEntity ToDomain(this PutVacancyRequest request, Guid id)
    {
        return new VacancyEntity {
            AccountId = request.AccountId,
            AccountLegalEntityId = request.AccountLegalEntityId,
            AdditionalQuestion1 = request.AdditionalQuestion1,
            AdditionalQuestion2 = request.AdditionalQuestion2,
            AdditionalTrainingDescription = request.AdditionalTrainingDescription,
            AnonymousReason = request.AnonymousReason,
            ApplicationInstructions = request.ApplicationInstructions,
            ApplicationMethod = request.ApplicationMethod,
            ApplicationUrl = request.ApplicationUrl,
            ApprenticeshipType = request.ApprenticeshipType,
            ApprovedDate = request.ApprovedDate,
            ClosedDate = request.ClosedDate,
            ClosingDate = request.ClosingDate,
            ClosureReason = request.ClosureReason,
            ContactEmail = request.Contact?.Email,
            ContactName = request.Contact?.Name,
            ContactPhone = request.Contact?.Phone,
            CreatedDate = request.CreatedDate,
            DeletedDate = request.DeletedDate,
            Description = request.Description,
            DisabilityConfident = request.DisabilityConfident,
            EmployerDescription = request.EmployerDescription,
            EmployerLocationInformation = request.EmployerLocationInformation,
            EmployerLocationOption = request.EmployerLocationOption,
            EmployerLocations = ApiUtils.SerializeOrNull(request.EmployerLocations),
            EmployerName = request.EmployerName,
            EmployerNameOption = request.EmployerNameOption,
            EmployerRejectedReason = request.EmployerRejectedReason,
            EmployerReviewFieldIndicators = ApiUtils.SerializeOrNull(request.EmployerReviewFieldIndicators),
            EmployerWebsiteUrl = request.EmployerWebsiteUrl,
            GeoCodeMethod = request.GeoCodeMethod,
            HasChosenProviderContactDetails = request.HasChosenProviderContactDetails,
            HasOptedToAddQualifications = request.HasOptedToAddQualifications,
            HasSubmittedAdditionalQuestions = request.HasSubmittedAdditionalQuestions,
            Id = id,
            LastUpdatedDate = request.LastUpdatedDate,
            LegalEntityName = request.LegalEntityName,
            LiveDate = request.LiveDate,
            NumberOfPositions = request.NumberOfPositions,
            OutcomeDescription = request.OutcomeDescription,
            OwnerType = request.OwnerType,
            ProgrammeId = request.ProgrammeId,
            ProviderReviewFieldIndicators = ApiUtils.SerializeOrNull(request.ProviderReviewFieldIndicators),
            Qualifications = ApiUtils.SerializeOrNull(request.Qualifications),
            ReviewCount = request.ReviewCount,
            ReviewRequestedDate = request.ReviewRequestedDate,
            ShortDescription = request.ShortDescription,
            Skills = ApiUtils.SerializeOrNull(request.Skills),
            SourceOrigin = request.SourceOrigin,
            SourceType = request.SourceType,
            SourceVacancyReference = request.SourceVacancyReference,
            StartDate = request.StartDate,
            Status = request.Status!.Value,
            SubmittedDate = request.SubmittedDate,
            ThingsToConsider = request.ThingsToConsider,
            Title = request.Title,
            TrainingDescription = request.TrainingDescription,
            TrainingProvider_Address = ApiUtils.SerializeOrNull(request.TrainingProvider?.Address),
            TrainingProvider_Name = request.TrainingProvider?.Name,
            TransferInfo = ApiUtils.SerializeOrNull(request.TransferInfo),
            Ukprn = (int?)request.TrainingProvider?.Ukprn,
            VacancyReference = request.VacancyReference,
            Wage_CompanyBenefitsInformation = request.Wage?.CompanyBenefitsInformation,
            Wage_Duration = request.Wage?.Duration,
            Wage_DurationUnit = request.Wage?.DurationUnit,
            Wage_FixedWageYearlyAmount = request.Wage?.FixedWageYearlyAmount,
            Wage_WageAdditionalInformation = request.Wage?.WageAdditionalInformation,
            Wage_WageType = request.Wage?.WageType,
            Wage_WeeklyHours = request.Wage?.WeeklyHours,
            Wage_WorkingWeekDescription = request.Wage?.WorkingWeekDescription
        };
    }

    public static VacancySummary ToSummary(this VacancyEntity entity)
    {
        return ToSummaryDto(entity);
    }
}