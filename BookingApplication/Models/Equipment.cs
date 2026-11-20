using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class Equipment
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    //navigation property
    public ICollection<Room> Rooms { get; set; } = [];
}