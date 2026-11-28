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
[Fact]
public async Task Validate_BookingStartsWhenAnotherEnds_ReturnsNoErrors()
  {
  // Existing 10-12 and new 12-14 only touch each other, so they do not overlap
  // Given
  Room room = MakeRoom(1);
  GivenBookingsInRoom(1, MakeBooking(5, 1, 10, 12));

  // When
List<BookingRuleError> errors = await CreateService().ValidateBookingAsync(MakeBooking(0, 1, 12, 14), room);

  // Then
  Assert.Empty(errors);
  }


// 3 - A group exactly as large as the rooms capacity is accepted.
[Fact]
public async Task Validate_PeopleEqualToCapacity_ReturnsNoErrors()
  {
  // Given
  Room room = MakeRoom(1, capacity: 4);
  GivenBookingsInRoom(1);

  // When
  List<BookingRuleError> errors = await CreateService().ValidateBookingAsync(MakeBooking(0, 1, 10, 12, people: 4), room);
  
  // Then
  Assert.Empty(errors);
  }

// 4 - Editing a booking does not make it clash with its own old time.
[Fact]
public async Task Validate_EditingBooking_IgnoresItselfInOverlapCheck()
  {
  // Given
  Room room = MakeRoom(1);
  GivenBookingsInRoom(1, MakeBooking(7, 1, 10, 12));
  Booking edited = MakeBooking(7, 1, 10, 11);

  // When
  List<BookingRuleError> errors = await CreateService().ValidateBookingAsync(edited, room, excludeBookingId: 7);

  // Then
  Assert.Empty(errors);
  }


// 5 - With no bookings, the search returns every room that fits the group, with the right details. 
[Fact]
public async Task SearchRooms_NoBookings_ReturnsAllRoomsThatFit()
  {
  // Given
  GivenSearchData(4, new List<Room> { MakeRoom(1, 6), MakeRoom(2, 10) }, new List<Booking>());

  // When
  List<AvailableRoomViewModel> result = await CreateService().SearchRooms(Day, At(10), At(12), 4);
  
  // Then
  Assert.Equal(2, result.Count);
  Assert.Equal("Room 1", result[0].Name);
  Assert.Equal(6, result[0].Capacity);
  Assert.Equal("Library", result[0].Location);
  }


// 6 - A booking in one room does not remove a different room from the search results. 
[Fact]
public async Task SearchRooms_BookingInAnotherRoom_KeepsTheRoomInTheResults()
  {
  // Room 2 is booked at the requested time, room 1 is free
  // Given
  GivenSearchData(2, new List<Room> { MakeRoom(1), MakeRoom(2) },
  new List<Booking> { MakeBooking(5, 2, 10, 12) });

  // When
  List<AvailableRoomViewModel> result = await CreateService().SearchRooms(Day, At(10), At(12), 2);

  // Then
  Assert.Single(result);
  Assert.Equal(1, result[0].Id);
  }


// Negative tests

// 7 - A booking that partly overlaps an existing one is rejected with an overlap error.
[Fact]
public async Task Validate_PartialOverlap_ReturnsOverlapError()
  {
  // Existing 10-12, new 11-13
  // Given
  Room room = MakeRoom(1);
  GivenBookingsInRoom(1, MakeBooking(5, 1, 10, 12));

  // When
  List<BookingRuleError> errors = await CreateService().ValidateBookingAsync(MakeBooking(0, 1, 11, 13), room);
  
  // Then
  BookingRuleError error = Assert.Single(errors);
  Assert.Equal(string.Empty, error.Field);
  Assert.Contains("already booked", error.Message);
  }

// 8 - A booking placed completely inside an existing one is rejected.
[Fact]
public async Task Validate_BookingInsideAnExistingBooking_ReturnsOverlapError()
  {
  // Existing 10-14, new 11-12 lies completely inside it
  // Given
  Room room = MakeRoom(1);
  GivenBookingsInRoom(1, MakeBooking(5, 1, 10, 14));

  // When
  List<BookingRuleError> errors = await CreateService().ValidateBookingAsync(MakeBooking(0, 1, 11, 12), room);

  // Then
  Assert.Single(errors);
  }

// 9 - A group larger than the rooms capacity is rejected with an error on People.
[Fact]
public async Task Validate_MorePeopleThanCapacity_ReturnsCapacityError()
  {
  // Given
  Room room = MakeRoom(1, capacity: 4);
  GivenBookingsInRoom(1);

  // When
  List<BookingRuleError> errors = await CreateService().ValidateBookingAsync(MakeBooking(0, 1, 10, 12, people: 5), room);

  // Then
  BookingRuleError error = Assert.Single(errors);
  Assert.Equal(nameof(Booking.People), error.Field);
  Assert.Contains("only fits 4 people", error.Message);
  }

// 10 - A booking that overlaps and is too large is rejected with both errors.
[Fact]
public async Task Validate_OverlapAndTooManyPeople_ReturnsBothErrors()
  {
  // Given
  Room room = MakeRoom(1, capacity: 4);
  GivenBookingsInRoom(1, MakeBooking(5, 1, 10, 12));
  
  // When
  List<BookingRuleError> errors = await CreateService().ValidateBookingAsync(MakeBooking(0, 1, 11, 13, people: 8), room);

  // Then
  Assert.Equal(2, errors.Count);
  }

// 11 - A room booked at the requested time is left out of the search results.
[Fact]
public async Task SearchRooms_RoomBookedAtRequestedTime_ExcludesTheRoom()
  {
  // Given
  GivenSearchData(2, new List<Room> { MakeRoom(1), MakeRoom(2) },
  new List<Booking> { MakeBooking(5, 1, 10, 12) });

  // When
  List<AvailableRoomViewModel> result = await CreateService().SearchRooms(Day, At(11), At(13), 2);

  // Then
  Assert.DoesNotContain(result, r => r.Id == 1);
  Assert.Single(result);
  }

// 12 - When every room is booked, the search returns an empty list.
[Fact]
public async Task SearchRooms_EveryRoomBooked_ReturnsEmptyList()
  {
  // Given
  GivenSearchData(2, new List<Room> { MakeRoom(1), MakeRoom(2) },
  new List<Booking> { MakeBooking(5, 1, 9, 22), MakeBooking(6, 2, 9, 22) });

  // When
  List<AvailableRoomViewModel> result = await CreateService().SearchRooms(Day, At(10), At(12), 2);

  // Then
  Assert.Empty(result);
  }
}