using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class RoomSearchViewModel : IValidatableObject
{
    public DateTime Date { get; set; } = DateTime.Today;
    [Range(typeof(TimeSpan),"09:00","22:00",ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
    public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0);
    [Range(typeof(TimeSpan),"09:00","22:00",ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
    public TimeSpan EndTime { get; set; } = new TimeSpan(10, 0, 0);

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
    [Range(1,10,ErrorMessage = "The number of people must be between 1 and 10")]
    public int People { get; set; } = 1;

    public List<AvailableRoomViewModel> AvailableRooms { get; set; } = [];
}

