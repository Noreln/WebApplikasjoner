using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class RoomSearchViewModel
{
    public DateTime Date { get; set; } = DateTime.Today;
    [Range(typeof(TimeSpan),"09:00","22:00",ErrorMessage = "Study rooms can only be booked between 09:00 and 22:00.")]
    public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0);
    [Range(1,10,ErrorMessage = "The number of people must be between 1 and 10")]
    public TimeSpan EndTime { get; set; } = new TimeSpan(10, 0, 0);

    public int People { get; set; } = 1;

    public List<AvailableRoomViewModel> AvailableRooms { get; set; } = [];
}

