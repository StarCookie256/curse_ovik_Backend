using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetProductsByBrandAsync(int brandId);
}
