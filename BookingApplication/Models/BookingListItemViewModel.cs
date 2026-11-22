namespace BookingApplication.Models;


public class BookingListItemViewModel
{
  public int Id {get; set;}
  public string RoomName {get; set; } = string.Empty;
  public string RoomLocation {get; set; } = string.Empty;
  public DateTime Date {get; set; }
  public TimeOnly StartTime {get; set; }
  public TimeOnly EndTime {get; set; }
  public int People {get; set; }
  public bool IsPast => Date.Date.Add(EndTime.ToTimeSpan()) < DateTime.Now;
}