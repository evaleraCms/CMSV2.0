using CMS.Core.Enums;

namespace CMS.Api.Models
{
    public class CMSValidationResult
    {

        public bool HasErrors => ValidationFailures.Any(q => q.HasErrors);
        public ValidationCollection ValidationFailures { get; set; } = new ValidationCollection();

        public CMSValidationResult() { }

        public CMSValidationResult(ICollection<ValidationProperty> validationFailures)
        {

            validationFailures = new ValidationCollection();
            foreach (var validationFailure in validationFailures)
            {
                ValidationFailures.Add(validationFailure);
            }

        }

    }

    public class ValidationCollection : List<ValidationProperty>
    {
        public void Add(string nonSpecificErrorMessage)
        {
            Add(new ValidationProperty("NonSpecificError", nonSpecificErrorMessage, ValidationRule.Unknown));
        }

    }

    public class NotFoundResult : CMSValidationResult
    {
        public NotFoundResult(string message = "Not Found")
        {
            ValidationFailures.Add(message);
        }
    }
}