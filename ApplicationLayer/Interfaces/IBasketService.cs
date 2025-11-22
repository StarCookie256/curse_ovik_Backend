using PerfumeryBackend.ApplicationLayer.DTO.Basket;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IBasketService
{
    Task<BasketPageDto?> GetBasketByCustomerId(int customerId);
    Task DeleteBasketItem(BasketDto item);
    Task AddBasketItem(BasketDto item);
    Task<int?> GetBasketItemsCount(int customerId);
}
