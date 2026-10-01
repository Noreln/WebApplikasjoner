using System.ComponentModel.DataAnnotations;
//Took inspiration from: https://medium.com/@syeedmdtalha/custom-validator-in-asp-net-core-mvc-beginner-friendly-8625d1178492 
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
