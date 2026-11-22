namespace BookingApplication.Models;

public class EquipmentListItemViewModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;

  public int RoomCount { get; set; }

  public bool IsInUse => RoomCount > 0;
}