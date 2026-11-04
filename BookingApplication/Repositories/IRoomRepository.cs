using BookingApplication.Models;

namespace BookingApplication.Repositories;

//Defines the data operations available for rooms.
public interface IRoomRepository
{
  Task<List<Room>> GetAllAsync();
  Task<Room?> GetByIdAsync(int id);
  Task AddAsync(Room room);
  Task UpdateAsync(Room room);
  Task DeleteAsync(Room room);
}