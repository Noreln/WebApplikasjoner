using Microsoft.AspNetCore.Mvc;
using BookingApplication.Models;
using BookingApplication.Repositories;

namespace BookingApplication.Controllers;

public class RoomController : Controller
{
  private readonly IRoomRepository _roomRepository;
  private readonly IEquipmentRepository _equipmentRepository;
  private readonly ILogger<RoomController> _logger;

  public RoomController(IRoomRepository roomRepository,
  IEquipmentRepository equipmentRepository,
  ILogger<RoomController> logger)
  {
    _roomRepository = roomRepository;
    _equipmentRepository = equipmentRepository;
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
      // The checkbox list is not releoaded here, since the database may be the problem
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
      Room? room = await _roomRepository.GetByIdAsync(id);
      if (room == null)
      {
        return NotFound();
      }
      return View(room);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load room {RoomId} for deletion", id);
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
}