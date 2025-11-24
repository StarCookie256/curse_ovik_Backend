using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class BasketRepository(PerfumeryDbContext context) : IBasketRepository
{
    public async Task<int> GetBasketIdByCustomerId(int customerId)
    {
        var basket = await context.Baskets
            .AsNoTracking()
            .FirstAsync(x => x.CustomerId == customerId);

        return basket.Id;
    }

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

    public async Task CreateBasketByCustomerId(Basket basket)
    {
        await context.Baskets.AddAsync(basket);
        await context.SaveChangesAsync();
    }

    public async Task ChangeBasketTotalPrice(int customerId, int productId, char operation)
    {
        Basket? basket = await context.Baskets
            .Include(x => x.BasketItems)
                .ThenInclude(bi => bi.ProductVariation)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);

        // Получить цену для конкретного ProductVariation
        ProductVariation targetItem = await context.ProductVariations
            .AsNoTracking()
            .FirstAsync(bi => bi.Id == productId);

        if (targetItem != null)
        {
            double? priceChange = targetItem.Price;
            if (operation == '+')
            {
                basket.TotalPrice += priceChange;
            }
            else if (operation == '-')
            {
                if (basket.TotalPrice > priceChange)
                {
                    basket.TotalPrice -= priceChange;
                }
                else
                {
                    basket.TotalPrice = 0;
                }
            }

            context.Entry(basket).Property(x => x.TotalPrice).IsModified = true;

            await context.SaveChangesAsync();
        }
    }
}
