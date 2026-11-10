using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using BookingApplication.Repositories;
using BookingApplication.Services;

namespace BookingApplication.Controllers;

public class HomeController : Controller
{
private readonly IBookingService _bookingService;

public HomeController(IBookingService bookingService)
  {
    _bookingService=bookingService; 
  }

public async Task<IActionResult> Index(RoomSearchViewModel model)
  {
    if(!ModelState.IsValid)
    {
    return View(model);        
    }
    List<AvailableRoomViewModel> rooms = await _bookingService.SearchRooms(model.Date,model.StartTime,model.EndTime,model.People);
    model.AvailableRooms=rooms;
    return View(model);
  }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult MyBookings()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpPost]
    public IActionResult SearchRooms(RoomSearchViewModel model)
    {
        if(!ModelState.IsValid)
        {
        return View("Index",model);    
        }
        return RedirectToAction("Index", "Home",new
        {
        Date=model.Date.ToString("yyyy-MM-dd"),
        StartTime=model.StartTime,
        EndTime=model.EndTime,
        People=model.People
        });
    }
    }



