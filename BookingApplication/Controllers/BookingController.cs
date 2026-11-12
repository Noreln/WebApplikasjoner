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

    var model = new BookingFormViewModel
    {
      RoomId = roomId,
      Date = date == default ? DateTime.Today : date,
      StartTime = startTime,
      EndTime = endTime,
      People = people == 0 ? 1 : people
    };
    FillRoomInfo(model, room);

    return View(model);
  }
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(BookingFormViewModel model)
  {
    try
    {
      Room? room = await _roomRepository.GetByIdAsync(model.RoomId);
      if (room == null)
      {
        return NotFound();
      }

      FillRoomInfo(model, room);

      // Stop early if the form input itself is invalid (required fields, ranges, end after start.)
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var booking = new Booking
      {
        RoomId = model.RoomId,
        BookedBy = model.BookedBy.Trim(),
        Date = model.Date,
        StartTime = model.StartTime,
        EndTime = model.EndTime,
        People = model.People
      };
      // Domain rules (overlap + capacity) are checked in BookingService
      var ruleErrors = await _bookingService.ValidateBookingAsync(booking, room);
      foreach (var error in ruleErrors)
      {
        ModelState.AddModelError(error.Field, error.Message);
      }
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      await _bookingRepository.AddAsync(booking);
      _logger.LogInformation("Booking created for room {RoomId} by {BookedBy}.", booking.RoomId, booking.BookedBy);
      return RedirectToAction("MyBookings", "Home", new { name = booking.BookedBy });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to save booking for room {RoomId}.", model.RoomId);
      ModelState.AddModelError(string.Empty, "Something went wrong while saving your booking. Please try again.");
      return View(model);
    }
  }

  private static void FillRoomInfo(BookingFormViewModel model, Room room)
  {
    model.RoomName = room.Name;
    model.RoomLocation = room.Location;
    model.RoomCapacity = room.Capacity;
  }
}