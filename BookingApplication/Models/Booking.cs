using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class Booking
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
  public TimeSpan StartTime { get; set; }

  [Required]
  public TimeSpan EndTime { get; set; }

  [Range(1, 12, ErrorMessage =  "Number of people must be between 1 and 12.")]
  public int People { get; set; } = 1;
}