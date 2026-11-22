using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class RoomSearchViewModel : IValidatableObject
{
    [DateCannotBeInThePast]
    public DateTime Date { get; set; } = DateTime.Today;
    public TimeOnly StartTime { get; set; } = new TimeOnly(9, 0, 0);
    public TimeOnly EndTime { get; set; } = new TimeOnly(10, 0, 0);
    //Took inspiration from: https://medium.com/@syeedmdtalha/custom-validator-in-asp-net-core-mvc-beginner-friendly-8625d1178492 
    [Range(1, 12, ErrorMessage = "The number of people must be between 1 and 12")]
    public int People { get; set; } = 1;
    public List<AvailableRoomViewModel> AvailableRooms { get; set; } = [];
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var opens = new TimeOnly(9, 0);
        var closes = new TimeOnly(22, 0);

        if (StartTime < opens || StartTime > closes)
        {
            yield return new ValidationResult(
              "Study rooms can only be booked between 09:00 and 22:00",
              new[] { nameof(StartTime) });
        }
        if (EndTime < opens || EndTime > closes)
        {
            yield return new ValidationResult(
              "Study rooms can only be booked between 09:00 and 22:00",
              new[] { nameof(EndTime) });
        }
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "The end time must be after the start time",
                new[] { nameof(EndTime) });
        }
    }

}

