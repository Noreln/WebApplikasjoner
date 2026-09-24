namespace BookingApplication.Models
{
    public class RoomSearchViewModel
    {
        public DateTime Date { get; set; } = DateTime.Today;

        public TimeSpan Time { get; set; } = new TimeSpan(9, 0, 0);

        public int People { get; set; } = 1;
    }
}