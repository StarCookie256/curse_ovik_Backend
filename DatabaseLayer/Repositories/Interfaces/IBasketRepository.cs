using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IBasketRepository
{
    Task<int> GetBasketIdByCustomerId(int customerId);
    Task ChangeBasketTotalPrice(int customerId, int productId, char operation);
    Task<Basket> GetBasketByCustomerId(int customerId);
    Task CreateBasketByCustomerId(Basket basket);
    //Task<int?> GetBasketItemsCount(int customerId);
}
