using System.ComponentModel.DataAnnotations;

public class DateCannotBeInThePastAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
    if(value is DateTime Date && Date<DateTime.Today)
        {
        return new ValidationResult("The date can't be earlier than the current date");
        }
        return ValidationResult.Success;  
    }
}
