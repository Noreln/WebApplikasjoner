using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookingApplication.Controllers;

public class BookingController : Controller
{
  private readonly RoomDbContext dbContext;
  private readonly ILogger<BookingController> logger;

  public BookingController (RoomDbContext dbContext, ILogger <BookingController> logger)
  {
    this.dbContext = dbContext;
    this.logger = logger;
  }

  public IActionResult Create(int roomId, DateTime date, TimeSpan startTime, TimeSpan endTime, int people)
  {
    Room? room = dbContext.Rooms.FirstOrDefault(r => r.Id == roomId);
    if (room == null)
    {
      return NotFound();
    }

    var booking = new Booking
    {
      RoomId = roomId,
      Date = date == default? DateTime.Today : date,
      StartTime = startTime,
      EndTime = endTime,
      People = people == 0 ? 1 : people
    };

    ViewBag.Room = room;
    return View(booking);
  }

  [HttpPost]
  public IActionResult Create(Booking booking)
  {
    Room? room = dbContext.Rooms.FirstOrDefault(r => r.Id == booking.RoomId);
    if (room == null)
    {
      return NotFound();
    }

    var sameDayBookings = dbContext.Bookings
        .Where(b => b.RoomId == booking.RoomId && b.Date == booking.Date)
        .ToList();

    bool overlaps = sameDayBookings.Any(b =>
    booking.StartTime < b.EndTime &&
    booking.EndTime > b.StartTime);

    if (overlaps)
    {
      ModelState.AddModelError(string.Empty, "This room is already booked for an overlapping time on that date/time.");
    }

    if(booking.People > room.Capacity)
    {
      ModelState.AddModelError(nameof(booking.People), $"This room only fits {room.Capacity} people.");
    }

    if(!ModelState.IsValid)
    {
      ViewBag.Room = room;
      return View(booking);
    }

    try
    {
      dbContext.Bookings.Add(booking);
      dbContext.SaveChanges();
      logger.LogInformation("Booking created for room {RoomId} by {BookedBy}.", booking.RoomId, booking.BookedBy);
      return RedirectToAction("MyBookings", "Home", new { name = booking.BookedBy });
    }
    catch(Exception ex)
    {
      logger.LogError(ex, "Failed to save booking for room {RoomId}.", booking.RoomId);
      ModelState.AddModelError(string.Empty, "Something went wrong while saving you booking. Please try again.");
      ViewBag.Room = room;
      return View(booking);
    }
  }
}