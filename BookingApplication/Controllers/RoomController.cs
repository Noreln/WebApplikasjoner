using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

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
  public IActionResult Create()
  {
    return View();
  }

  [HttpPost]
  public IActionResult Create(Room room)
  {
    dbContext.Rooms.Add(room);
    dbContext.SaveChanges();
    return RedirectToAction("Index");
  }

  public IActionResult Edit(int id)
  {
    Room? room = dbContext.Rooms.FirstOrDefault(r => r.Id == id);
    if(room == null)
    {
      return NotFound();
    }
    return View(room);
  }

  [HttpPost]
  public IActionResult Edit(int id, Room updatedRoom)
  {
    Room? room = dbContext.Rooms.FirstOrDefault(r => r.Id == id);
    if (room == null)
    {
      return NotFound();
    }

    room.Name = updatedRoom.Name;
    dbContext.SaveChanges();
    return RedirectToAction("Index");
  }

  public IActionResult Delete(int id)
  {
    Room? room = dbContext.Rooms.FirstOrDefault(r => r.Id == id);
    if(room == null)
    {
      return NotFound();
    }
    return View(room);
  }

  [HttpPost]
  public IActionResult DeleteConfirmed(int id)
  {
    Room? room = dbContext.Rooms.FirstOrDefault(r => r.Id == id);
    if (room == null)
    {
      return NotFound();
    }

    dbContext.Rooms.Remove(room);
    dbContext.SaveChanges();
    return RedirectToAction("Index");
  }

}