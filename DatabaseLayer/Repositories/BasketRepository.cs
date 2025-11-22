using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class BasketRepository(PerfumeryDbContext context) : IBasketRepository
{
    public async Task<Basket> GetBasketByCustomerId(int customerId) =>
        await context.Baskets
            .AsNoTracking()
            .Include(x => x.BasketItems)
            .ThenInclude(bi => bi.ProductVariation)
                .ThenInclude(pv => pv.Product)
            .Include(x => x.BasketItems)
            .ThenInclude(bi => bi.ProductVariation)
                .ThenInclude(pv => pv.Category)
            .FirstAsync(x => x.CustomerId == customerId);
}
