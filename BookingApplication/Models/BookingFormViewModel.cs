using System.ComponentModel.DataAnnotations;
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
  [Range(typeof(TimeOnly), "09:00", "22:00", ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
  public TimeOnly StartTime { get; set; }

  [Required]
  [Range(typeof(TimeOnly), "09:00", "22:00", ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
  public TimeOnly EndTime { get; set; }

  [Range(1, 12, ErrorMessage = "Number of people must be between 1 and 12.")]
  public int People { get; set; } = 1;

  public string RoomName { get; set; } = string.Empty;
  public string RoomLocation { get; set; } = string.Empty;
  public int RoomCapacity { get; set; }

  public bool IsEdit => Id != 0;

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (EndTime <= StartTime)
    {
      yield return new ValidationResult(
          "The end time must be after the start time",
          new[] { nameof(EndTime) }
      );
    }
  }


}

