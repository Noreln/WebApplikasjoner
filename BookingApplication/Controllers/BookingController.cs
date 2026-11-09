using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using BookingApplication.Repositories;

namespace BookingApplication.Controllers;

public class BookingController : Controller
{
  private readonly IRoomRepository _roomRepository;
  private readonly IBookingRepository _bookingRepository;
  private readonly ILogger<BookingController> _logger;

  public BookingController(IRoomRepository roomRepository,
                          IBookingRepository bookingRepository,
                          ILogger<BookingController> logger)
  {
    _roomRepository = roomRepository;
    _bookingRepository = bookingRepository;
    _logger = logger;
  }

  public async Task<IActionResult> Create(int roomId, DateTime date, TimeSpan startTime, TimeSpan endTime, int people)
  {
    Room? room = await _roomRepository.GetByIdAsync(roomId);
    if (room == null)
    {
      return NotFound();
    }

    var booking = new Booking
    {
      RoomId = roomId,
      Date = date == default ? DateTime.Today : date,
      StartTime = startTime,
      EndTime = endTime,
      People = people == 0 ? 1 : people
    };

    ViewBag.Room = room;
    return View(booking);
  }

  [HttpPost]
  public async Task<IActionResult> Create(Booking booking)
  {
    Room? room = await _roomRepository.GetByIdAsync(booking.RoomId);
    if (room == null)
    {
      return NotFound();
    }

    var sameDayBookings = await _bookingRepository.GetByRoomAndDateAsync(booking.RoomId, booking.Date);

    bool overlaps = sameDayBookings.Any(b =>
    booking.StartTime < b.EndTime &&
    booking.EndTime > b.StartTime);

    if (overlaps)
    {
      ModelState.AddModelError(string.Empty, "This room is already booked for an overlapping time on that date/time.");
    }

    if (booking.People > room.Capacity)
    {
      ModelState.AddModelError(nameof(booking.People), $"This room only fits {room.Capacity} people.");
    }

    if (!ModelState.IsValid)
    {
      ViewBag.Room = room;
      return View(booking);
    }

    try
    {
      await _bookingRepository.AddAsync(booking);
      _logger.LogInformation("Booking created for room {RoomId} by {BookedBy}.", booking.RoomId, booking.BookedBy);
      return RedirectToAction("MyBookings", "Home", new { name = booking.BookedBy });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to save booking for room {RoomId}.", booking.RoomId);
      ModelState.AddModelError(string.Empty, "Something went wrong while saving your booking. Please try again.");
      ViewBag.Room = room;
      return View(booking);
    }
  }
}