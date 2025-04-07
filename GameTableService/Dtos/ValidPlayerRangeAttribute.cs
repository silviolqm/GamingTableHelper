using System.ComponentModel.DataAnnotations;
using GameTableService.Dtos;

public class ValidPlayerRangeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dto = validationContext.ObjectInstance as GameTableCreateDto;

        if (dto == null)
        {
            return ValidationResult.Success;
        }

        if (dto.MaxPlayers <= dto.MinPlayers)
        {
            return new ValidationResult("MaxPlayers must be greater than MinPlayers.");
        }

        return ValidationResult.Success;
    }
}