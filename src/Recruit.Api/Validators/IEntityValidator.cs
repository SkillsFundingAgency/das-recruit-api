using FluentValidation;
using FluentValidation.Results;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.Validators;

public interface IEntityValidator<in T, in TRules>
{
    EntityValidationResult Validate(T entity, TRules rules);
}

public sealed class EntityValidator<T, TRules> : IEntityValidator<T, TRules> where TRules : struct, IComparable, IConvertible, IFormattable 
{
    private readonly AbstractValidator<T> _validator;

    public EntityValidator(AbstractValidator<T> fluentValidator)
    {
        _validator = fluentValidator;
    }

    public EntityValidationResult Validate(T entity, TRules rules)
    {
        return ValidateEntity(entity, rules).Result;
    }

    private async Task<EntityValidationResult> ValidateEntity(T entity, TRules rules)
    {
        var context = new ValidationContext<T>(entity);

        context.RootContextData.Add(ValidationConstants.ValidationsRulesKey, rules);

        var fluentResult = await _validator.ValidateAsync(context);

        if (!fluentResult.IsValid)
        {
            return CreateValidationErrors(fluentResult);
        }

        return new EntityValidationResult();
    }

    private EntityValidationResult CreateValidationErrors(ValidationResult fluentResult)
    {
        var newResult = new EntityValidationResult();

        if (fluentResult.IsValid == false && fluentResult.Errors.Count > 0)
        {
            foreach(var fluentError in fluentResult.Errors)
            {
                newResult.Errors.Add(new EntityValidationError(ParseForRuleId(fluentError.CustomState), fluentError.PropertyName, fluentError.ErrorMessage, fluentError.ErrorCode));
            }
        }

        return newResult;
    }

    private long ParseForRuleId(object customState)
    {
        if (customState == null)
            throw new ArgumentNullException(nameof(customState), "Fluent Error should have CustomState property set to the RuleId");

        try
        {
			return Convert.ToInt64(customState);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Unexpected value for customState. Expecting a long", nameof(customState), ex);
        }
    }
}
    
public class EntityValidationResult
{
    public EntityValidationResult()
    {
        Errors = new List<EntityValidationError>();
    }

    public bool HasErrors => Errors?.Count() > 0;

    public IList<EntityValidationError> Errors { get; set; }

    public static EntityValidationResult FromFluentValidationResult(ValidationResult fluentResult)
    {
        var result = new EntityValidationResult();

        if (fluentResult.IsValid == false && fluentResult.Errors.Count > 0)
        {
            foreach (var fluentError in fluentResult.Errors)
            {
                result.Errors.Add(new EntityValidationError(long.Parse(fluentError.ErrorCode), fluentError.PropertyName, fluentError.ErrorMessage, fluentError.ErrorCode));
            }
        }

        return result;
    }
}

public class EntityValidationError
{
    public EntityValidationError(long ruleId, string propertyName, string errorMessage, string errorCode)
    {
        RuleId = ruleId;
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public long RuleId { get; set; }
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorCode { get; set; }
}