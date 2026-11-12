using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using BookingApplication.Repositories;
using BookingApplication.Services;

namespace BookingApplication.Controllers;

public class BookingController : Controller
{
  private readonly IBookingService _bookingService;
  private readonly IRoomRepository _roomRepository;
  private readonly IBookingRepository _bookingRepository;
  private readonly ILogger<BookingController> _logger;

  public BookingController(IBookingService bookingService,
                          IRoomRepository roomRepository,
                          IBookingRepository bookingRepository,
                          ILogger<BookingController> logger)
  {
    _bookingService = bookingService;
    _roomRepository = roomRepository;
    _bookingRepository = bookingRepository;
    _logger = logger;
  }
  public async Task<IActionResult> Create(int roomId, DateTime date, TimeOnly startTime, TimeOnly endTime, int people)
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
    // Domain rules (overlap + capacity) are checked in BookingService
    var ruleErrors = await _bookingService.ValidateBookingAsync(booking, room);
    foreach (var error in ruleErrors)
    {
      ModelState.AddModelError(error.Field, error.Message);
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