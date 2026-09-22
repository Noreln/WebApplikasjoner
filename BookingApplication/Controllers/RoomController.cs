using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace BookingApplication.Controllers;

public class RoomController : Controller
{
  private readonly RoomDbContext dbContext;

  public RoomController(RoomDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

    public IActionResult Index()
  {
    
List<Room> rooms = dbContext.Rooms.ToList();
return View(rooms);

  }
  

}