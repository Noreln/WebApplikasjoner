using BookingApplication.Models;
using BookingApplication.Repositories;
using BookingApplication.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BookingApplication.Tests;

// Unit tests for BookingService (room search, overlap rule and capacity rule).
// The repositories are replaced with Moq mocks, so no database is used.
// 6 positive tests and 6 negative tests.
// The helper methods at the top build test data and set up what the mocks return. 
// Each [Fact] below then calls the service once and asserts the result.
public class BookingServiceTests
{

// One fixed day, so the tests do not depend on todays date
private static readonly DateTime Day = new(2030, 1, 15);

private readonly Mock<IRoomRepository> _rooms = new();

private readonly Mock<IBookingRepository> _bookings = new();

private BookingService CreateService() =>
new(_rooms.Object, _bookings.Object, NullLogger<BookingService>.Instance);

private static TimeOnly At(int hour) => new(hour, 0);

private static Room MakeRoom(int id, int capacity = 6) =>
new() { Id = id, Name = $"Room {id}", Location = "Library", Capacity = capacity, Description = "Test room" };

private static Booking MakeBooking(int id, int roomId, int startHour, int endHour, int people = 2) =>
new()
{
  Id = id,
  RoomId = roomId,
  BookedBy = "Anna",
  Date = Day,
  StartTime = At(startHour),
  EndTime = At(endHour),
  People = people
};

// The repository returns these bookings for the room and day that ValidateBookingAsync asks for
private void GivenBookingsInRoom(int roomId, params Booking[] existing) =>
_bookings.Setup(b => b.GetByRoomAndDateAsync(roomId, Day)).ReturnsAsync(existing.ToList());

// THe repositories return these rooms and bookings for SearchRooms

private void GivenSearchData(int people, List<Room> rooms, List<Booking> bookingsThatDay)
{
  _rooms.Setup(r => r.GetRoomsByMinCapacity(people)).ReturnsAsync(rooms);
  _bookings.Setup(b => b.GetByDateAsync(Day)).ReturnsAsync(bookingsThatDay);
  
}


// Positive tests: 
// 1 - A valid booking in a free room is accepted with no errors.
[Fact]
public async Task Validate_NoExistingBookings_ReturnsNoErrors()
{
  // Given
  Room room = MakeRoom(1, capacity: 6);
  GivenBookingsInRoom(1);

  // When
  List<BookingRuleError> errors = await CreateService().ValidateBookingAsync(MakeBooking(0, 1, 10, 12), room);

  // Then
  Assert.Empty(errors);
}

// 2 - A booking that starts exactly when another one ends is accepted.


// 3 - A group exactly as large as the rooms capacity is accepted.


// 4 - Editing a booking does not make it clash with its own old time.


// 5 - With no bookings, the search returns every room that fits the group, with the right details. 


// 6 - A booking in one room does not remove a different room from the search results. 



// Negative tests

// 7 - A booking that partly overlaps an existing one is rejected with an overlap error.


// 8 - A booking placed completely inside an existing one is rejected.


// 9 - A group larger than the rooms capacity is rejected with an error on People.


// 10 - A booking that overlaps and is too large is rejected with both errors.


// 11 - A room booked at the requested time is left out of the search results.


// 12 - When every room is booked, the search returns an empty list.

}