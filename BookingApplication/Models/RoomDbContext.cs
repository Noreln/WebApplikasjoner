using Microsoft.EntityFrameworkCore;

namespace SessionBooking.Models;

public class RoomDbContext : DbContext
{
  public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options)
  {
    Database.EnsureCreated();
  }


  public DbSet<Room> Rooms {get; set;}
}