using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class RoomSearchViewModel : IValidatableObject
{
    [DateCannotBeInThePast]
    public DateTime Date { get; set; } = DateTime.Today;
    [Range(typeof(TimeOnly),"09:00","22:00",ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
    public TimeOnly StartTime { get; set; } = new TimeOnly(9, 0, 0);
    [Range(typeof(TimeOnly),"09:00","22:00",ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
    public TimeOnly EndTime { get; set; } = new TimeOnly(10, 0, 0);
    //Took inspiration from: https://medium.com/@syeedmdtalha/custom-validator-in-asp-net-core-mvc-beginner-friendly-8625d1178492 
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
    [Range(1,12,ErrorMessage = "The number of people must be between 1 and 12")]
    public int People { get; set; } = 1;

    public List<AvailableRoomViewModel> AvailableRooms { get; set; } = [];
}

