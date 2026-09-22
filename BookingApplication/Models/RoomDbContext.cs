using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Models;

public class RoomDbContext : DbContext
{
  public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options)
  {
  }
    public DbSet<Room> Rooms {get; set;}
  }


  
