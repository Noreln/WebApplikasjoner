using BookingApplication.Models;

namespace BookingApplication.Repositories;

// Defines the operations available for bookings.
public interface IBookingRepository
{
  Task<Booking?> GetByIdAsync(int id);
  Task<List<Booking>> GetByRoomAndDateAsync(int roomId, DateTime date);
  Task<List<Booking>> GetByBookedByAsync(string bookedBy);
  Task AddAsync(Booking booking);
  Task UpdateAsync(Booking booking);
  Task DeleteAsync(Booking booking);
}