using BookingApplication.Models;

namespace BookingApplication.Services;

public interface IBookingService
{
    Task<List<AvailableRoomViewModel>> SearchRooms(DateTime date, TimeOnly startTime, TimeOnly endTime, int people);
    Task<List<BookingRuleError>> ValidateBookingAsync(Booking booking, Room room, int? excludeBookingId = null);
}
