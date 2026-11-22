using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
//using BookingApplication.Models.Validation;
//usingen over blir uncommented etter at arvin har gjort sine deler, da blir den satt i bruk

namespace BookingApplication.Models;
/// <summary>
/// Form data for creating and editing a booking, plus room info for display.
/// </summary>
public class BookingFormViewModel : IValidatableObject
{

  public int Id { get; set; }

  [Required]
  public int RoomId { get; set; }
  [Required(ErrorMessage = "Please enter your name. ")]
  [StringLength(60)]
  public string BookedBy { get; set; } = string.Empty;

  [Required]
  [DateCannotBeInThePast]
  public DateTime Date { get; set; } = DateTime.Today;

  [Required]
  public TimeOnly StartTime { get; set; }

  [Required]
  public TimeOnly EndTime { get; set; }

  [Range(1, 12, ErrorMessage = "Number of people must be between 1 and 12.")]
  public int People { get; set; } = 1;

  public string RoomName { get; set; } = string.Empty;
  public string RoomLocation { get; set; } = string.Empty;
  public int RoomCapacity { get; set; }

  public bool IsEdit => Id != 0;

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    var opens = new TimeOnly(9, 0);
    var closes = new TimeOnly(22, 0);

    if (StartTime < opens || StartTime > closes)
    {
      yield return new ValidationResult(
        "Study rooms can only be booked between 09:00 and 22:00",
        new[]{ nameof(StartTime)});
    }
    if (EndTime < opens || EndTime > closes)
    {
      yield return new ValidationResult(
        "Study rooms can only be booked between 09:00 and 22:00",
        new[] {nameof(EndTime)});
    }
    if (EndTime <= StartTime)
    {
      yield return new ValidationResult(
          "The end time must be after the start time",
          new[] { nameof(EndTime) });
    }
  }
}

