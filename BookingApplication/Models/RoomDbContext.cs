using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace BookingApplication.Models;

public class RoomDbContext : DbContext
{
  public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options)
  {
  }
  public DbSet<Room> Rooms { get; set; }
  public DbSet<Booking> Bookings { get; set; }
  public DbSet<Equipment> Equipments { get; set; }
  public DbSet<RoomEquipment> RoomEquipments { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Equipment>()
    .HasIndex(e => e.Name)
    .IsUnique();
    modelBuilder.Entity<Room>()
    .HasMany(r => r.Equipments)
    .WithMany(e => e.Rooms)
    .UsingEntity<RoomEquipment>(
    eq => eq.HasOne(re => re.Equipment).WithMany().HasForeignKey("EquipmentId"),
    rm => rm.HasOne(re => re.Room).WithMany().HasForeignKey("RoomId"));
  }
}



