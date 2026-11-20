using BookingApplication.Models;
using BookingApplication.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookingApplication.Controllers;

public class EquipmentController : Controller
{
  private readonly IEquipmentRepository _equipmentRepository;
  private readonly ILogger<EquipmentController> _logger;

  public EquipmentController(IEquipmentRepository equipmentRepository,
  ILogger<EquipmentController> logger)

  {
    _equipmentRepository = equipmentRepository;
    _logger = logger;
  }

  public async Task<IActionResult> Index()
  {
    try
    {
      List<Equipment> equipment = await _equipmentRepository.GetAllAsync();

      var model = equipment
      .Select(e => new EquipmentListItemViewModel
      {
        Id = e.Id,
        Name = e.Name,
        RoomCount = e.Rooms.Count
      })
      .ToList();

      return View(model);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load the equipment list.");
      return RedirectToAction("Error", "Home");
    }
  }

  public IActionResult Create()
  {
    return View(new EquipmentFormViewModel());
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(EquipmentFormViewModel model)
  {
    if (!ModelState.IsValid)
    {
      return View(model);
    }

    try
    {
      if (await _equipmentRepository.NameExistsAsync(model.Name))
      {
        ModelState.AddModelError(nameof(model.Name), "Equipment with this name already exists.");
        return View(model);
      }

      var equipment = new Equipment { Name = model.Name.Trim() };
      await _equipmentRepository.CreateAsync(equipment);

      _logger.LogInformation("Equipment {EquipmentId} ({Name}) Created.", equipment.Id, equipment.Name);
      return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to create equipment {Name}.", model.Name);
      ModelState.AddModelError(string.Empty, "Something went wrong while saving. Please try again.");
      return View(model);
    }
  }

  public async Task<IActionResult> Edit(int id)
  {
    try
    {
      Equipment? equipment = await _equipmentRepository.GetByIdAsync(id);
      if (equipment == null)
      {
        return NotFound();
      }

      var model = new EquipmentFormViewModel
      {
        Id = equipment.Id,
        Name = equipment.Name
      };
      return View(model);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load equipment {EquipmentId} for editing", id);
      return RedirectToAction("Error", "Home");
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, EquipmentFormViewModel model)
  {
    if (id != model.Id)
    {
      return BadRequest();
    }

    if (!ModelState.IsValid)
    {
      return View(model);
    }

    try
    {
      Equipment? equipment = await _equipmentRepository.GetByIdAsync(id);
      if (equipment == null)
      {
        return NotFound();
      }

      if (await _equipmentRepository.NameExistsAsync(model.Name, id))
      {
        ModelState.AddModelError(nameof(model.Name), "Equipment with this name already exists.");
        return View(model);
      }

      equipment.Name = model.Name.Trim();
      await _equipmentRepository.UpdateAsync(equipment);

      _logger.LogInformation("Equipment {EquipmentId} renamed to {Name}", equipment.Id, equipment.Name);
      return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to Update equipment {EquipmentId}.", id);
      ModelState.AddModelError(string.Empty, "Something went wrong while saving. Please try again.");
      return View(model);
    }
  }

  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      Equipment? equipment = await _equipmentRepository.GetByIdAsync(id);
      if (equipment == null)
      {
        return NotFound();
      }

      if (await _equipmentRepository.IsInUseAsync(id))
      {
        ModelState.AddModelError(string.Empty, "This equipment is used by one or more rooms. Remove it from those rooms first.");
      }

      var model = new EquipmentFormViewModel
      {
        Id = equipment.Id,
        Name = equipment.Name
      };
      return View(model);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load equipment {EquipmentId} for deletion", id);
      return RedirectToAction("Error", "Home");
    }
  }

  [HttpPost]
  [ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    try
    {
      Equipment? equipment = await _equipmentRepository.GetByIdAsync(id);
      if (equipment == null)
      {
        return NotFound();
      }

      if (await _equipmentRepository.IsInUseAsync(id))
      {
        _logger.LogWarning("Blocked deletion of equipment {EquipmentId} because it is in user.", id);
        ModelState.AddModelError(string.Empty, "This equipment is used by one or more rooms. Remove it from those rooms first.");
        return View(new EquipmentFormViewModel { Id = equipment.Id, Name = equipment.Name });
      }

      await _equipmentRepository.DeleteAsync(equipment);
      _logger.LogInformation("Equipment {EquipmentId} ({Name}) deleted.", equipment.Id, equipment.Name);
      return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to delete equipment {EquipmentId}.", id);
      return RedirectToAction("Error", "Home");
    }
  }
}