using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using BookingApplication.Repositories;

namespace BookingApplication.Controllers;

public class RoomController : Controller
{
  private readonly IRoomRepository _roomRepository;

  public RoomController(IRoomRepository roomRepository)
  {
    _roomRepository = roomRepository;
  }

  public async Task<IActionResult> Index()
  {
    List<Room> rooms = await _roomRepository.GetAllAsync();
    return View(rooms);
  }
  public IActionResult Create()
  {
    return View();
  }

  [HttpPost]
  public async Task<IActionResult> Create(Room room)
  {
    await _roomRepository.AddAsync(room);
    return RedirectToAction("Index");
  }

  public async Task<IActionResult> Edit(int id)
  {
    Room? room = await _roomRepository.GetByIdAsync(id);
    if (room == null)
    {
      return NotFound();
    }
    return View(room);
  }

  [HttpPost]
  public async Task<IActionResult> Edit(int id, Room updatedRoom)
  {
    Room? room = await _roomRepository.GetByIdAsync(id);
    if (room == null)
    {
      return NotFound();
    }


    room.Name = updatedRoom.Name;
    room.Capacity = updatedRoom.Capacity;
    room.Location = updatedRoom.Location;
    room.Description = updatedRoom.Description;
    await _roomRepository.UpdateAsync(room);
    return RedirectToAction("Index");
  }

  public async Task<IActionResult> Delete(int id)
  {
    Room? room = await _roomRepository.GetByIdAsync(id);
    if (room == null)
    {
      return NotFound();
    }
    return View(room);
  }

  [HttpPost]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    Room? room = await _roomRepository.GetByIdAsync(id);
    if (room == null)
    {
      return NotFound();
    }

    await _roomRepository.DeleteAsync(room);
    return RedirectToAction("Index");
  }

}