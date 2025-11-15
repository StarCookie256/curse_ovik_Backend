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

    public IQueryable<Product> GetAllProductsAsync() =>
        context.Products
        .AsQueryable();

    public async Task<Product> GetProductByIdAsync(int id) =>
        await context.Products
        .AsNoTracking()
        .FirstAsync(x => x.Id == id);

    public async Task<int> GetProductsCountAsync() => 
        await context.Products
        .CountAsync();
}
