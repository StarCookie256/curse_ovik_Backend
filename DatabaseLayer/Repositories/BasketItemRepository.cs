using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class BasketItemRepository(PerfumeryDbContext context) : IBasketItemsRepository
{
    public async Task AddBasketItem(BasketItem item)
    {
        bool alreadyHave = await context.BasketItems
            .AsNoTracking()
            .AnyAsync(x => x == item);
            
        if (alreadyHave)
        {
            BasketItem? basketItem = await context.BasketItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x == item);
            
            BasketItem? updatedItem = basketItem;
            updatedItem.Stock++;
            context.Entry(basketItem).CurrentValues.SetValues(updatedItem);
            await context.SaveChangesAsync();
        }
        else
        {
            await context.BasketItems.AddAsync(item);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteBasketItem(BasketItem item)
    {
        bool alreadyHave = await context.BasketItems
            .AsNoTracking()
            .AnyAsync(x => x == item);

        if (alreadyHave)
        {
            BasketItem? basketItem = await context.BasketItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x == item);

            if (basketItem.Stock > 1)
            {
                BasketItem? updatedItem = basketItem;
                updatedItem.Stock--;
                context.Entry(basketItem).CurrentValues.SetValues(updatedItem);
                await context.SaveChangesAsync();
            }
            else
            {
                context.BasketItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task<int?> GetBasketItemsCount(int customerId)
    {
        List<BasketItem> basketItems = await context.BasketItems
            .AsNoTracking()
            .Include(x => x.Basket)
            .Where(x => x.Basket.CustomerId == customerId)
            .ToListAsync();

        int? count = 0;
        foreach (var basketItem in basketItems)
        {
            count += basketItem.Stock;
        }

        return count;
    }
}
