using BookingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Repositories;

// all db access gets handled for bookings using entity framework
public class BookingRepository : IBookingRepository
{
  private readonly RoomDbContext _db;

  public BookingRepository(RoomDbContext db)
  {
    _db = db;
  }
  public async Task<List<Booking>> GetByRoomAndDateAsync(int roomId, DateTime date)
  {
    return await _db.Bookings
   .Where(b => b.RoomId == roomId && b.Date == date)
   .ToListAsync();
  }
  public async Task<List<Booking>> GetByBookedByAsync(string bookedBy)
  {
    return await _db.Bookings
    .Include(b => b.Room)
    .Where(b => b.BookedBy == bookedBy)
    .OrderBy(b => b.Date)
    .ToListAsync();
  }
  public async Task<Booking?> GetByIdAsync(int id)
  {
    return await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id);
  }
  public async Task AddAsync(Booking booking)
  {
    _db.Bookings.Add(booking);
    await _db.SaveChangesAsync();
  }
  public async Task UpdateAsync(Booking booking)
  {
    _db.Bookings.Update(booking);
    await _db.SaveChangesAsync();
  }
  public async Task DeleteAsync(Booking booking)
  {
    _db.Bookings.Remove(booking);
    await _db.SaveChangesAsync();
  }

}