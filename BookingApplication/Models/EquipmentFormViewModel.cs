using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models;

public class EquipmentFormViewModel
{
  public int Id { get; set; }

  [Required(ErrorMessage = "Please enter a name")]
  [StringLength(50, ErrorMessage = "The name can be at most 50 characters.")]
  [Display(Name = "Equipment name")]
  public string Name { get; set; } = string.Empty;

  public bool isEdit => Id != 0;
}