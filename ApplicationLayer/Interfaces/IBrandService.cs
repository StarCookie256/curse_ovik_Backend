using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IBrandService
{
    Task<Brand> GetBrandByNameAsync(string name);
    Task<Brand> GetBrandByIdAsync(int id);
    Task<List<Brand>> GetBrandsAsync();
}
