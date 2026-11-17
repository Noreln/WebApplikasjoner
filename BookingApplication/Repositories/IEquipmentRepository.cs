using BookingApplication.Models;

namespace BookingApplication.Repositories;

public interface IEquipmentRepository
{
    Task<List<Equipment>> GetAllAsync();
    Task<Equipment?> GetByIdAsync(int id);
    Task CreateAsync(Equipment equipment);
    Task UpdateAsync(Equipment equipment);
    Task DeleteAsync(Equipment equipment);
    Task<bool> IsInUseAsync(int id);
}