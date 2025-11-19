using PerfumeryBackend.ApplicationLayer.DTO.Products;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetProductsByBrandAsync(int brandId);
    IQueryable<Product> GetProductsSearchAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<Product> GetProductForPageByIdAsync(int id);
    Task<int> GetProductsCountAsync();
}
