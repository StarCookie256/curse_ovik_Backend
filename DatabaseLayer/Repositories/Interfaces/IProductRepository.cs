using PerfumeryBackend.ApplicationLayer.DTO.Products;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetProductsByBrandAsync(int brandId);
    Task<List<Product>> GetProductsSearchAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<Product> GetProductForPageByIdAsync(int id);
    Task<int> GetProductsCountAsync();
}
