using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using BookingApplication.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookingApplication.Controllers;

public class RoomController : Controller
{
  private readonly IRoomRepository _roomRepository;
  private readonly IEquipmentRepository _equipmentRepository;
  private readonly ILogger<RoomController> _logger;
  private readonly IBookingRepository _bookingRepository;

  public RoomController(IRoomRepository roomRepository,
  IEquipmentRepository equipmentRepository,
  IBookingRepository bookingRepository,
  ILogger<RoomController> logger)
  {
    _roomRepository = roomRepository;
    _equipmentRepository = equipmentRepository;
    _bookingRepository = bookingRepository;
    _logger = logger;
  }

  public async Task<IActionResult> Index()
  {
    try
    {
      List<Room> rooms = await _roomRepository.GetAllAsync();
      return View(rooms);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load the room list.");
      return RedirectToAction("Error", "Home");
    }
  }

  public async Task<IActionResult> Create()
  {
    try
    {
      var model = new RoomFormViewModel();
      await FillEquipmentOptionsAsync(model);
      return View(model);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load the create room form.");
      return RedirectToAction("Error", "Home");
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(RoomFormViewModel model)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        await FillEquipmentOptionsAsync(model);
        return View(model);
      }

      List<Equipment> selectedEquipment = await _equipmentRepository.GetByIdsAsync(model.SelectedEquipmentIds);

      var room = new Room
      {
        Name = model.Name.Trim(),
        Capacity = model.Capacity,
        Location = model.Location.Trim(),
        Description = model.Description?.Trim(),
        Equipments = selectedEquipment
      };

      await _roomRepository.AddAsync(room);
      _logger.LogInformation("Room {RoomId} ({Name}) created with {EquipmentCount} equipment.",
      room.Id, room.Name, selectedEquipment.Count);
      return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to create room {Name}.", model.Name);
      ModelState.AddModelError(string.Empty, "Something went wrong while saving the room. Please try again.");
      // The checkbox list is not reloaded here, since the database may be the problem
      return View(model);
    }
  }

  public async Task<IActionResult> Edit(int id)
  {
    try
    {
      Room? room = await _roomRepository.GetByIdWithEquipmentAsync(id);
      if (room == null)
      {
        return NotFound();
      }

      var model = new RoomFormViewModel
      {
        Id = room.Id,
        Name = room.Name,
        Capacity = room.Capacity,
        Location = room.Location,
        Description = room.Description,
        //Tick the boxes for the equipment the room already has
        SelectedEquipmentIds = room.Equipments.Select(e => e.Id).ToList()
      };
      await FillEquipmentOptionsAsync(model);

      return View(model);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load room {RoomId} for editing.", id);
      return RedirectToAction("Error", "Home");
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, RoomFormViewModel model)
  {
    if (id != model.Id)
    {
      return BadRequest();
    }

    try
    {
      Room? room = await _roomRepository.GetByIdWithEquipmentAsync(id);
      if (room == null)
      {
        return NotFound();
      }

      if (!ModelState.IsValid)
      {
        await FillEquipmentOptionsAsync(model);
        return View(model);
      }

      room.Name = model.Name.Trim();
      room.Capacity = model.Capacity;
      room.Location = model.Location.Trim();
      room.Description = model.Description?.Trim();


      List<Equipment> removed = room.Equipments
      .Where(e => !model.SelectedEquipmentIds.Contains(e.Id))
      .ToList();
      foreach (Equipment equipment in removed)
      {
        room.Equipments.Remove(equipment);
      }

      List<int> currentIds = room.Equipments.Select(e => e.Id).ToList();
      List<Equipment> added = await _equipmentRepository.GetByIdsAsync(
        model.SelectedEquipmentIds.Except(currentIds));
      foreach (Equipment equipment in added)
      {
        room.Equipments.Add(equipment);
      }

      await _roomRepository.UpdateAsync(room);
      _logger.LogInformation("Room {RoomId} updated: {AddedCount} equipment added, {RemovedCount} removed.",
      room.Id, added.Count, removed.Count);
      return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to update room {RoomId}.", id);
      ModelState.AddModelError(string.Empty, "Something went wrong while saving the room. Please try again.");
      return View(model);
    }
  }

  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      RoomDeleteViewModel? model = await BuildDeleteModelAsync(id);
      if (model == null)
      {
        return NotFound();
      }

      if (!model.CanDelete)
      {
        ModelState.AddModelError(string.Empty,
        $"This room has {model.UpcomingBookingCount} upcoming booking(s) and can't be deleted.");
      }
      return View(model);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load room {RoomId} for deletion.", id);
      return RedirectToAction("Error", "Home");
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    try
    {
      Room? room = await _roomRepository.GetByIdAsync(id);
      if (room == null)
      {
        return NotFound();
      }

      int upcomingCount = await _bookingRepository.CountUpcomingByRoomAsync(id);
      if (upcomingCount > 0)
      {
        _logger.LogWarning("Blocked deletion of room {RoomId} because it has {Count} upcoming bookings.",
        id, upcomingCount);
        ModelState.AddModelError(string.Empty,
        $"This room has {upcomingCount} upcoming booking(s) and can't be deleted.");
        return View("Delete", await BuildDeleteModelAsync(id));
      }

      await _roomRepository.DeleteAsync(room);
      _logger.LogInformation("Room {RoomId} ({Name}) deleted.", room.Id, room.Name);
      return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to delete room {RoomId}.", id);
      return RedirectToAction("Error", "Home");
    }
  }

  private async Task FillEquipmentOptionsAsync(RoomFormViewModel model)
  {
    List<Equipment> allEquipment = await _equipmentRepository.GetAllAsync();
    model.AvailableEquipment = allEquipment
    .Select(e => new EquipmentListItemViewModel { Id = e.Id, Name = e.Name })
    .ToList();
  }

  private async Task<RoomDeleteViewModel?> BuildDeleteModelAsync(int id)
  {
    Room? room = await _roomRepository.GetByIdAsync(id);
    if (room == null)
    {
      return null;
    }

    return new RoomDeleteViewModel
    {
      Id = room.Id,
      Name = room.Name,
      Location = room.Location,
      UpcomingBookingCount = await _bookingRepository.CountUpcomingByRoomAsync(id)
    };
  }
}