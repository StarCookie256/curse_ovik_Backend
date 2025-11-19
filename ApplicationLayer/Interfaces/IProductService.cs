using PerfumeryBackend.ApplicationLayer.DTO.Products;
using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IProductService
{
    public Task<List<ProductDto>> GetProductsOfDayAsync();
    public Task<ProductPageDto> GetProductForPageByIdAsync(int productId);
    public Task<List<ProductDto>> GetProductsByBrand(int brandId); 
    public Task<PagedResult<ProductDto>> GetProductsBySearch(ProductSearchDto searchDto);
}
