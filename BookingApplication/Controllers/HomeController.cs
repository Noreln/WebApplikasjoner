using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using BookingApplication.Repositories;
using BookingApplication.Services;

namespace BookingApplication.Controllers;

public class HomeController : Controller
{
  private readonly IBookingService _bookingService;
  private readonly IBookingRepository _bookingRepository;
  private readonly ILogger<HomeController> _logger;


  public HomeController(
    IBookingService bookingService,
    IBookingRepository bookingRepository,
    ILogger<HomeController> logger)
  {
    _bookingService = bookingService;
    _bookingRepository = bookingRepository;
    _logger = logger;
  }

  public async Task<IActionResult> Index(RoomSearchViewModel model)
  {
    if (!ModelState.IsValid)
    {
      return View(model);
    }
    List<AvailableRoomViewModel> rooms = await _bookingService.SearchRooms(model.Date, model.StartTime, model.EndTime, model.People);
    model.AvailableRooms = rooms;
    return View(model);
  }

  public IActionResult Privacy()
  {
    return View();
  }

  public async Task<IActionResult> MyBookings(string? name)
  {
    var model = new MyBookingsViewModel { Name = name };
    if (!model.HasSearched)
    {
      return View(model);
    }
    try
    {
      var bookings = await _bookingRepository.GetByBookedByAsync(name!.Trim());
      model.Bookings = bookings
      .Select(b => new BookingListItemViewModel
      {
        Id = b.Id,
        RoomName = b.Room?.Name ?? "Unknown room",
        RoomLocation = b.Room?.Location ?? "Unknown location",
        Date = b.Date,
        StartTime = b.StartTime,
        EndTime = b.EndTime,
        People = b.People
      })
      .ToList();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load bookings for {Name}, ", name);
      model.ErrorMessage = "Please try again";
    }
    return View(model);
  }


  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
  [HttpPost]
  public IActionResult SearchRooms(RoomSearchViewModel model)
  {
    if (!ModelState.IsValid)
    {
      return View("Index", model);
    }
    return RedirectToAction("Index", "Home", new
    {
      Date = model.Date.ToString("yyyy-MM-dd"),
      StartTime = model.StartTime,
      EndTime = model.EndTime,
      People = model.People
    });
  }
}



