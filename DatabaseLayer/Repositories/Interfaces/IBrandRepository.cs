using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IBrandRepository
{
    Task<Brand> GetBrandByIdAsync(int id);
    Task<Brand> GetBrandByNameAsync(string name);
    Task<List<Brand>> GetBrandsAsync();
}
