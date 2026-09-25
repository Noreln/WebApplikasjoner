using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;

namespace BookingApplication.Controllers;

public class HomeController : Controller
{
public IActionResult Index()
{
    var model = new RoomSearchViewModel
    {
        AvailableRooms =
        [
            new AvailableRoomViewModel
            {
                Id = 1,
                Name = "Meeting Room A",
                Capacity = 8,
                Location = "Building A · 2nd floor",
                Description = "Small meeting room with display."
            },

            new AvailableRoomViewModel
            {
                Id = 2,
                Name = "Conference Room B",
                Capacity = 20,
                Location = "Building A · 1st floor",
                Description = "Large conference room with projector."
            },

            new AvailableRoomViewModel
            {
                Id = 3,
                Name = "Meeting Room C",
                Capacity = 6,
                Location = "Building B · 3rd floor",
                Description = "Quiet meeting room with whiteboard."
            }
        ]
    };

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
        // We'll implement the actual room search later.

        return View("SearchResults", model);
    }

    
}



