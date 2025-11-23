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
        var basket = await context.Baskets
            .AsNoTracking()
            .Include(x => x.BasketItems)
                .ThenInclude(bi => bi.ProductVariation)
            .FirstAsync(x => x.CustomerId == customerId);

        // Получить цену для конкретного ProductVariation
        var targetItem = basket.BasketItems
            .FirstOrDefault(bi => bi.ProductVariation.Id == productId);

        if (targetItem != null)
        {
            if(operation == '+')
            {
                basket.TotalPrice += targetItem.ProductVariation.Price;
            }
            else
            {
                // Проверка, чтобы цена не ушла в минус
                basket.TotalPrice -= targetItem.ProductVariation.Price;
            }

            context.Entry(basket).Property(x => x.TotalPrice).IsModified = true;

            await context.SaveChangesAsync();
        }
    }
}
