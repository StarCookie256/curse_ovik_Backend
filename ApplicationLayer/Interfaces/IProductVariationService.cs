using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IProductVariationService
{
    public Task<VolumesAndPricesDto> GetVolumesAndPricesByProductAsync(int productId);
    public Task<List<ProductVariation>> GetVariationsByProductAsync(int productId);
    public Task<List<string>> GetCategoriesByProductAsync(int productId);
}
