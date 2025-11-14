using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class ProductVariationsRepository(PerfumeryDbContext context) : IProductVariationsRepository
{
    public async Task<VolumesAndPricesDto> GetVolumesAndPricesByProductAsync(int productId)
    {
        List<ProductVariation> variations = await context.ProductVariations
            .Where(x => x.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();

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
