using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models;

public class VacancySnapshot
{
    public Guid Id { get; set; }
    public string ApplicationInstructions { get; set; }
    public string Description { get; set; }
    public ContactDetail EmployerContact { get; set; }
    public string EmployerDescription { get; set; }
    public List<Address> EmployerLocations { get; set; }
    public string? EmployerLocationInformation { get; set; }
    public string EmployerName { get; set; }
    public EmployerNameOption? EmployerNameOption { get; set; }
    public string OutcomeDescription { get; set; }
    public ContactDetail ProviderContact { get; set; }
    public List<Qualification> Qualifications { get; set; }
    public string? ShortDescription { get; set; }
    public List<string> Skills { get; set; }
    public string ThingsToConsider { get; set; }
    public string Title { get; set; }
    public string TrainingDescription { get; set; }
    public string AdditionalTrainingDescription { get; set; }
    public Wage Wage { get; set; }
    public string AdditionalQuestion1 { get; set; }
    public string AdditionalQuestion2 { get; set; }
}