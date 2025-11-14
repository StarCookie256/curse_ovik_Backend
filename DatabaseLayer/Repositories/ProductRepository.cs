using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class ProductRepository(PerfumeryDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetProductsByBrandAsync(int brandId) =>
        await context.Products
            .Where(x => x.BrandId == brandId)
            .AsNoTracking()
            .ToListAsync();
}
