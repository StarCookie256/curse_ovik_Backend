using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IBasketItemsRepository
{
    Task<int?> GetBasketItemsCount(int customerId);
    Task AddBasketItem(BasketItem item);
    Task DeleteBasketItem(BasketItem item);
}
