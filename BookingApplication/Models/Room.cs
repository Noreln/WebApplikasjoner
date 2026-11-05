using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class Room
{
    public int Id { get; set; }
    [Required]
    [Display(Name="Room name")]
    public string Name { get; set; } = string.Empty;
    [Range(1,12,ErrorMessage = "Capacity must be between 1 and 12 people")]
    public int Capacity { get; set; }
    [Required]
    [StringLength(50)]
    public string Location { get; set; } = string.Empty;
    [StringLength(200)]
    public string? Description { get; set; }
    //Navigation property
    public ICollection<Booking> Bookings { get; set;}= new List<Booking>();
}