namespace BookingApplication.Models;

/// <summary>
/// One room in the list, including the names of its equipment
/// </summary>
public class RoomListItemViewModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Location { get; set; } = string.Empty;
  public string? Description { get; set; }
  public int Capacity { get; set; }

  public List<string> EquipmentNames { get; set; } = [];
}