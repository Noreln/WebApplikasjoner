using BookingApplication.Models;
using BookingApplication.Repositories;

namespace BookingApplication.Services;

public class BookingService : IBookingService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IRoomRepository roomRepository, IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    private bool HasOverlap(IEnumerable<Booking> bookings, int roomId, DateTime date,
                            TimeOnly startTime, TimeOnly endTime, int? excludeBookingId = null)
    {
        return bookings.Any(b =>
        b.Id != excludeBookingId && // ignore the booking being edited
        b.RoomId == roomId &&
        b.Date == date &&
        b.StartTime < endTime &&
        b.EndTime > startTime);
    }

    public async Task<List<AvailableRoomViewModel>> SearchRooms(DateTime date, TimeOnly startTime, TimeOnly endTime, int people)
    {
        var rooms = await _roomRepository.GetRoomsByMinCapacity(people);
        var bookings = await _bookingRepository.GetByDateAsync(date);
        return rooms
        .Where(r => !HasOverlap(bookings, r.Id, date, startTime, endTime))
        .Select(r => new AvailableRoomViewModel
        {
            Id = r.Id,
            Name = r.Name,
            Location = r.Location,
            Description = r.Description,
            Capacity = r.Capacity
        })
        .ToList();
    }

    /// <summary>
    /// Checks the booking against the domain rules (no overlap, within capacity).
    /// Returns an empty list if the booking is valid
    /// </summary>
    public async Task<List<BookingRuleError>> ValidateBookingAsync(Booking booking, Room room, int? excludeBookingId = null)
    {
        var errors = new List<BookingRuleError>();
        // Rule 1: no overlapping booking for the same room on the same date
        var sameDayBookings = await _bookingRepository.GetByRoomAndDateAsync(booking.RoomId, booking.Date);
        if (HasOverlap(sameDayBookings, booking.RoomId, booking.Date,
                        booking.StartTime, booking.EndTime, excludeBookingId))
        {
            errors.Add(new BookingRuleError(string.Empty, "This room is already booked for an overlapping time on that date."));
        }
        // Rule 2: the number of people must fit the room
        if (booking.People > room.Capacity)
        {
            errors.Add(new BookingRuleError(nameof(Booking.People), $"This room only fits {room.Capacity} people."));
        }
        return errors;
    }
}
