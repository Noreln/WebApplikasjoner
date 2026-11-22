using BookingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Data;

/// <summary>
///  Fills an empty database with example rooms and equipment,
/// so the app can be tested right after the first start.
/// </summary>
public static class SeedData
{
  public static async Task InitializeAsync(RoomDbContext db)
  {
    // Only seed an empty database, so existing data is never overwritten
    if (await db.Rooms.AnyAsync() || await db.Equipments.AnyAsync())
    {
      return;
    }

    var projector = new Equipment { Name = "Projector" };
    var whiteboard = new Equipment { Name = "Whiteboard" };
    var screen = new Equipment { Name = "Screen" };
    var videoCall = new Equipment { Name = "Video call kit" };

    db.Rooms.AddRange(
      new Room
      {
        Name = "Group Room 1",
        Capacity = 4,
        Location = "Library, 1st floor",
        Description = "Quiet room with a window.",
        Equipments = [whiteboard]
      },
      new Room
      {
        Name = "Group Room 2",
        Capacity = 6,
        Location = "Library, 2nd floor",
        Description = "Good for group projects.",
        Equipments = [whiteboard, screen]
      },
      new Room
      {
        Name = "Meeting Room A",
        Capacity = 10,
        Location = "Main building, 3rd floor",
        Description = "large room for presentations",
        Equipments = [whiteboard, screen, videoCall]
      },
      new Room
      {
        Name = "Study Booth",
        Capacity = 2,
        Location = "Library, 1st floor",
        Description = null,
        Equipments = []
  });

  await db.SaveChangesAsync();
  }
}