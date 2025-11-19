using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.ApplicationLayer.DTO.Products;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class ProductRepository(PerfumeryDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetProductsByBrandAsync(int brandId) =>
        await context.Products
            .Include(p => p.Brand)
            .Where(x => x.BrandId == brandId)
            .AsNoTracking()
            .ToListAsync();

    public IQueryable<Product> GetProductsSearchAsync() =>
         context.Products
            .Include(p => p.Brand)
            .AsNoTracking()
            .AsQueryable();

    public async Task<Product> GetProductForPageByIdAsync(int id) =>
        await context.Products
        .Include(x => x.Brand)
        .Include(x => x.Country)
        .AsNoTracking()
        .FirstAsync(x => x.Id == id);

    public async Task<Product> GetProductByIdAsync(int id) =>
        await context.Products
        .Include(x => x.Brand)
        .AsNoTracking()
        .FirstAsync(x => x.Id == id);

    public async Task<int> GetProductsCountAsync() => 
        await context.Products
        .CountAsync();
}
