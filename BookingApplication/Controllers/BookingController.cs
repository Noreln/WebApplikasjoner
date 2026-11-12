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

  /// <summary>
  /// Creates a new booking after checking input and domain rules.
  /// </summary>

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

  public async Task<IActionResult> Edit(int id)
  {
    try
    {
      Booking? booking = await _bookingRepository.GetByIdAsync(id);
      if (booking == null)
      {
        return NotFound();
      }

      Room? room = await _roomRepository.GetByIdAsync(booking.RoomId);
      if (room == null)
      {
        return NotFound();
      }

      var model = new BookingFormViewModel
      {
        Id = booking.Id,
        RoomId = booking.RoomId,
        BookedBy = booking.BookedBy,
        Date = booking.Date,
        StartTime = booking.StartTime,
        EndTime = booking.EndTime,
        People = booking.People
      };
      FillRoomInfo(model, room);

      return View(model);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load booking {BookingId} for editing.", id);
      return RedirectToAction("Index", "Home");
    }
  }

  /// <summary>
  /// Saves changes to an existing booking after checking the domain rules.
  /// </summary>
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, BookingFormViewModel model)
  {
    // the URL id must match the hidden id in the form
    if (id != model.Id)
    {
      return BadRequest();
    }

    try
    {
      Booking? booking = await _bookingRepository.GetByIdAsync(id);
      if (booking == null)
      {
        return NotFound();
      }

      // Use the booking's real room, not the roomId posted from the form
      Room? room = await _roomRepository.GetByIdAsync(booking.RoomId);
      if (room == null)
      {
        return NotFound();
      }

      FillRoomInfo(model, room);
      // Stop early if the form input itself is invalid
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      // Update the tracked booking instead of creating a new one
      booking.BookedBy = model.BookedBy.Trim();
      booking.Date = model.Date;
      booking.StartTime = model.StartTime;
      booking.EndTime = model.EndTime;
      booking.People = model.People;

      //  exclude this booking from the overlap check
      var ruleErrors = await _bookingService.ValidateBookingAsync(booking, room, booking.Id);
      foreach (var error in ruleErrors)
      {
        ModelState.AddModelError(error.Field, error.Message);
      }
      // Show the form again if a domain rule was broken
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      await _bookingRepository.UpdateAsync(booking);
      _logger.LogInformation("Booking {BookingId} updated by {BookedBy}.", booking.Id, booking.BookedBy);
      return RedirectToAction("MyBookings", "Home", new { name = booking.BookedBy });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to save booking {BookingId}.", id);
      ModelState.AddModelError(string.Empty, "Something went wrong while saving your changes. Please try again.");
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