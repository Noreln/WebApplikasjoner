using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class Booking: IValidatableObject
{
  public int Id { get; set; }

  [Required]
  public int RoomId { get; set; }

  public Room? Room { get; set; }

  [Required(ErrorMessage = "Please enter your name. ")]
  [StringLength(60)]
  public string BookedBy { get; set; } = string.Empty;

  [Required]
  [DateCannotBeInThePast]
  public DateTime Date { get; set; } = DateTime.Today;

  [Required]
  [Range(typeof(TimeSpan),"09:00","22:00",ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
  public TimeSpan StartTime { get; set; }

  [Required]
  [Range(typeof(TimeSpan),"09:00","22:00",ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
  public TimeSpan EndTime { get; set; }

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
    if(EndTime<=StartTime)
        {
            yield return new ValidationResult(
                "The end time must be after the start time",
                new[] { nameof(EndTime) }
            );
        }    
    }

  [Range(1, 12, ErrorMessage =  "Number of people must be between 1 and 12.")]
  public int People { get; set; } = 1;
}