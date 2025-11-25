using PerfumeryBackend.ApplicationLayer.DTO.Basket;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IBasketService
{
    Task<int> GetBasketIdByCustomerId(int customerId);
    Task CreateBasketByCustomerId(int customerId);
    Task<BasketPageDto?> GetBasketByCustomerId(int customerId);
    Task DeleteBasketItem(BasketDto item);
    Task AddBasketItem(BasketDto item);
    Task<int?> GetBasketItemsCount(int customerId);
    Task ClearBasket(int customerId);
}
