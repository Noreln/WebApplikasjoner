using BookingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly RoomDbContext _db;

    public EquipmentRepository(RoomDbContext db)
    {
        _db = db;
    }
    public async Task<List<Equipment>> GetAllAsync()
    {
        return await _db.Equipments
        .OrderBy(e=> e.Name)
        .ToListAsync();
    }
    public async Task<Equipment?> GetByIdAsync(int id)
    {
        return await _db.Equipments.FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task CreateAsync(Equipment equipment)
    {
        _db.Equipments.Add(equipment);
        await _db.SaveChangesAsync();
    }
    public async Task UpdateAsync(Equipment equipment)
    {
        _db.Equipments.Update(equipment);
        await _db.SaveChangesAsync();
    }
    public async Task DeleteAsync(Equipment equipment)
    {
        _db.Equipments.Remove(equipment);
        await _db.SaveChangesAsync();
    }
    public async Task<bool> IsInUseAsync(int id)
    {
        return await _db.RoomEquipments.AnyAsync(re => re.EquipmentId == id);
    }
}

