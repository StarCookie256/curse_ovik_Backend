using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface IProductVariationsRepository
{
    Task<VolumesAndPricesDto> GetVolumesAndPricesByProductAsync(int productId);
}
