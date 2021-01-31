using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pixit.Shared.Utils
{
    public class NestedObjectValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext vc)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateObject(value, new ValidationContext(value), results, true);
            if (results.Count > 0) return new ValidationResult("Niepoprawny obiekt.");
                
            return ValidationResult.Success;
        }
    }
}