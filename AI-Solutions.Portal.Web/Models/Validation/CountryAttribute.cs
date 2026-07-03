using System.ComponentModel.DataAnnotations;
using AI_Solutions.Portal.Web.Models;

namespace AI_Solutions.Portal.Web.Models.Validation;

public class CountryAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string country || string.IsNullOrWhiteSpace(country))
        {
            return ValidationResult.Success;
        }

        if (Countries.All.Contains(country.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("Please select a valid country from the list.");
    }
}
