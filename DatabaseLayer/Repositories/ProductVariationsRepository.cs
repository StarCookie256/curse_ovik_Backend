using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class ProductVariationsRepository(PerfumeryDbContext context) : IProductVariationsRepository
{
    public async Task<List<ProductVariation>> GetVariationsByProductAsync(int productId) =>
        await context.ProductVariations
            .Include(x => x.Category)
            .Where(x => x.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
}
