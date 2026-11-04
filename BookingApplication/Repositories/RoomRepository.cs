using BookingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Repositories;


public class RoomRepository : IRoomRepository
{
  private readonly RoomDbContext _db;

  public RoomRepository(RoomDbContext db)
  {
    _db = db;
  }

  public async Task<List<Room>> GetAllAsync()
  {
    return await _db.Rooms.ToListAsync();
  }

  public async Task<Room?> GetByIdAsync(int id)
  {
    return await _db.Rooms.FirstOrDefaultAsync(r => r.Id == id);
  }

  public async Task AddAsync (Room room)
  {
    _db.Rooms.Add(room);
    await _db.SaveChangesAsync();
  }

  public async Task UpdateAsync (Room room)
  {
    _db.Rooms.Update(room);
    await _db.SaveChangesAsync();
  }

  public async Task DeleteAsync (Room room)
  {
    _db.Rooms.Remove(room);
    await _db.SaveChangesAsync();
  }

  
}