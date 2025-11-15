using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class ProductVariationService(IProductVariationsRepository productVariationsRepository) : IProductVariationService
{
    public async Task<List<string>> GetCategoriesByProductAsync(int productId)
    {
        List<ProductVariation> variations = await productVariationsRepository.GetVariationsByProductAsync(productId);

        List<string> categories = new();

        foreach (var variation in variations)
        {
            categories.Add(variation.Category.Name);
        }

        return categories;
    }

    public async Task<List<ProductVariation>> GetVariationsByProductAsync(int productId) =>
        await productVariationsRepository.GetVariationsByProductAsync(productId);

    public async Task<VolumesAndPricesDto> GetVolumesAndPricesByProductAsync(int productId)
    {
        List<ProductVariation> variations = await productVariationsRepository.GetVariationsByProductAsync(productId);

        double? fVolume = variations[0].Volume;
        double? sVolume = variations[0].Volume;
        double? fPrice = variations[0].Price;
        double? sPrice = variations[0].Price;
        foreach (var variation in variations)
        {
            if (variation.Volume < fVolume)
                fVolume = variation.Volume;
            else if (variation.Volume > sVolume)
                sVolume = variation.Volume;

            if (variation.Price < fPrice)
                fPrice = variation.Price;
            else if (variation.Price > sPrice)
                sPrice = variation.Price;
        }

        return new VolumesAndPricesDto(
            FVolume: fVolume.GetValueOrDefault(),
            SVolume: sVolume.GetValueOrDefault(),
            FPrice: fPrice.GetValueOrDefault(),
            SPrice: sPrice.GetValueOrDefault()
        );
    }
}
