using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class BasketItemRepository(PerfumeryDbContext context) : IBasketItemsRepository
{
    public async Task AddBasketItem(BasketItem item)
    {
        // Сначала проверяем существование корзины и продукта
        bool basketExists = await context.Baskets.AnyAsync(b => b.Id == item.BasketId);
        bool productExists = await context.ProductVariations.AnyAsync(pv => pv.Id == item.ProductVariationId);

        if (!basketExists || !productExists)
            throw new Exception("Basket or ProductVariation not found");

        // Ищем существующий элемент
        var existingItem = await context.BasketItems
            .FirstOrDefaultAsync(x => x.BasketId == item.BasketId && x.ProductVariationId == item.ProductVariationId);

        if (existingItem != null)
        {
            existingItem.Stock++;
        }
        else
        {
            item.Stock = 1;
            await context.BasketItems.AddAsync(item);
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteBasketItem(BasketItem item)
    {
        bool alreadyHave = await context.BasketItems
            .AsNoTracking()
            .AnyAsync(x => x.BasketId == item.BasketId && x.ProductVariationId == item.ProductVariationId);

        if (alreadyHave)
        {
            BasketItem? basketItem = await context.BasketItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BasketId == item.BasketId && x.ProductVariationId == item.ProductVariationId);

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
