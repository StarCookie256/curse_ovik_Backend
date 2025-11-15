using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IProductVariationsRepository
{
    Task<List<ProductVariation>> GetVariationsByProductAsync(int productId);
}
