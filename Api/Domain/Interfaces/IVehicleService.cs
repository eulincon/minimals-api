using minimals_api.Domain.DTOs;
using minimals_api.Domain.Entities;

namespace minimals_api.Domain.Interfaces;

public interface IVehicleService
{
    List<Vehicle>? All(int? page = 1, string? nome = null, string? marca = null);
    Vehicle? FindById(int id);
    void Add(Vehicle vehicle);
    void Update(Vehicle vehicle);
    void Delete(Vehicle vehicle);
}