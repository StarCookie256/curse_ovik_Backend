using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IBasketRepository
{
    Task<Basket> GetBasketByCustomerId(int customerId);
    //Task<int?> GetBasketItemsCount(int customerId);
}
