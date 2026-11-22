namespace BookingApplication.Models;

public class RoomDeleteViewModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Location { get; set; } = string.Empty;

  public int UpcomingBookingCount { get; set; }

  public bool CanDelete => UpcomingBookingCount == 0;
}