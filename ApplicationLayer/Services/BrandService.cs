using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class BrandService(IBrandRepository brandRepository) : IBrandService
{
    public async Task<Brand> GetBrandByIdAsync(int id) =>
        await brandRepository.GetBrandByIdAsync(id);

    public async Task<Brand> GetBrandByNameAsync(string name) =>
        await brandRepository.GetBrandByNameAsync(name);

    public async Task<List<Brand>> GetBrandsAsync() =>
        await brandRepository.GetBrandsAsync();
}
