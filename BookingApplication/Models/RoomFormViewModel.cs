using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class RoomFormViewModel
{
  public int Id { get; set; }

  [Required(ErrorMessage = "Please enter a room name.")]
  [StringLength(50, ErrorMessage = "The name can be at most 50 characters")]
  [Display(Name = "Room name")]
  public string Name { get; set; } = string.Empty;

  [Range(1, 12, ErrorMessage = "Capacity must be between 1 and 12 people")]
  public int Capacity { get; set; } = 1;

  [Required(ErrorMessage = "Please enter a location")]
  [StringLength(50, ErrorMessage = "The location can be at most 50 characters")]
  public string Location { get; set; } = string.Empty;

  [StringLength(200, ErrorMessage = "The description can be at most 200 characters.")]
  public string? Description { get; set; }

  [Display(Name = "Equipment")]
  public List<int> SelectedEquipmentIds { get; set; } = [];
  public List<EquipmentListItemViewModel> AvailableEquipment { get; set; } = [];

  public bool IsEdit => Id != 0;
}