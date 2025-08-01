using CMS.Core.Enums;
using JetBrains.Annotations;
using System.Xml.Schema;

namespace CMS.Api.Models
{
    public class ValidationProperty
    {
        public ValidationProperty([InvokerParameterName, NotNull] string propertyName,
            [NotNull, ItemNotNull] ICollection<string> errorMassages,
            ValidationRule validationRule, Severity severirty = Severity.Error)
        { 
            propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            errorMassages = errorMassages ?? throw new ArgumentNullException(nameof(errorMassages));
            AddValidationRule(validationRule);
        }
        public ValidationProperty([InvokerParameterName, NotNull] string propertyName,
           [NotNull, ItemNotNull] string errorMassage,
           ValidationRule validationRule, Severity severirty = Severity.Error)
        {
            propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            AddValidationRule(validationRule);
            AddError(errorMassage);
            FailereType = severirty;
        }
        public ValidationProperty([InvokerParameterName, NotNull] string propertyName,           
           ValidationRule validationRule)
        {
            propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            AddValidationRule(validationRule);
           
        }
        public ValidationProperty([InvokerParameterName, NotNull] string propertyName,    
        ICollection<ValidationRule> validationRules)
        {
            propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
           
            AddValidationRule(validationRules);
        }


        [NotNull]
        public ICollection<ValidationRule> ValidationRules { get; } = new List<ValidationRule>();
        [NotNull,ItemNotNull]
        public ICollection<string> ErrorMassages { get; } = new List<string>();
        public Severity FailereType { get; set; } = Severity.Error;
        public bool HasErrors => ErrorMassages.Any();
        [NotNull]
        public string PropertyName { get; } 
        public void AddError(string errorMessage) { 
        
            ErrorMassages.Add(errorMessage ?? throw new ArgumentNullException(nameof(errorMessage)));
        }

        public void AddValidationRule(ValidationRule validationRule)
        {
            if (validationRule == ValidationRule.Unknown)
            {
                throw new ArgumentException("Validation rule cannot be Unknown.", nameof(validationRule));
            }
            ValidationRules.Add(validationRule);
        }

        public void AddValidationRule(ICollection<ValidationRule> validationRules)
        {
            foreach (var rule in validationRules)
            {
                ValidationRules.Add(rule);
            }
            
        }

    }
}
