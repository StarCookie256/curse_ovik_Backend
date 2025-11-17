using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class BrandRepository(PerfumeryDbContext context) : IBrandRepository
{
    public async Task<Brand> GetBrandByIdAsync(int id) =>
        await context.Brands
            .AsNoTracking()
            .FirstAsync(x => x.Id == id);


    public async Task<Brand> GetBrandByNameAsync(string name) =>
        await context.Brands
            .AsNoTracking()
            .FirstAsync(x => x.Name == name);

    public async Task<List<Brand>> GetBrandsAsync() =>
        await context.Brands
            .AsNoTracking()
            .ToListAsync();

}
