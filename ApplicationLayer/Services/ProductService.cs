using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class ProductService(
    IProductRepository productRepository) : IProductService
{
    public async Task<List<Product>> GetProductsByBrand(int brandId) =>
        await productRepository.GetProductsByBrandAsync(brandId);

    public Task<Product> GetProductByIdAsync(int productId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetProductsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetProductsOfDayAsync()
    {
        throw new NotImplementedException();
    }
}
