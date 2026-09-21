using Microsoft.EntityFrameworkCore;

namespace SessionBooking.Models;

public class ItemDbContext : DbContext
{
  public ItemDbContext(DbContextOptions<ItemDbContext> options) : base(options)
  {
    Database.EnsureCreated();
  }


  public DbSet<Room> Rooms {get; set;}
}