using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IProductService
{
    public Task<List<Product>> GetProductsAsync();
    public Task<List<Product>> GetProductsOfDayAsync();
    public Task<Product> GetProductByIdAsync(int productId);
    public Task<List<Product>> GetProductsByBrand(int brandId); 
    
}
