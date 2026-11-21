using BookingApplication.Models;


namespace BookingApplication.Repositories;

/// <summary>
/// Defines the data opertations available for equipment
/// </summary>
public interface IEquipmentRepository
{
    Task<List<Equipment>> GetAllAsync();
    Task<Equipment?> GetByIdAsync(int id);
    Task CreateAsync(Equipment equipment);
    Task UpdateAsync(Equipment equipment);
    Task DeleteAsync(Equipment equipment);
    Task<bool> IsInUseAsync(int id);

    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task<List<Equipment>> GetByIdsAsync(IEnumerable<int> ids);
}