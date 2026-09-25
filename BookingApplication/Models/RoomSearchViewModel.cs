namespace BookingApplication.Models;

public class RoomSearchViewModel
{
    public DateTime Date { get; set; } = DateTime.Today;

    public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0);

    public TimeSpan EndTime { get; set; } = new TimeSpan(10, 0, 0);

    public int People { get; set; } = 1;

    public List<AvailableRoomViewModel> AvailableRooms { get; set; } = [];
}

