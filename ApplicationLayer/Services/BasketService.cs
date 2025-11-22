using PerfumeryBackend.ApplicationLayer.DTO.Basket;
using PerfumeryBackend.ApplicationLayer.DTO.BasketItems;
using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class BasketService(
    IBasketRepository basketRepository,
    IBasketItemsRepository basketItemRepository) : IBasketService
{
    public async Task AddBasketItem(BasketDto item)
    { 
        await basketItemRepository.AddBasketItem(new BasketItem 
        {
            Id = item.BasketId,
            BasketId = item.BasketItemId,
            ProductVariationId = item.ProductVariationId,
            Stock = item.Stock
        }); 
    }

    public async Task DeleteBasketItem(BasketDto item)
    {
        await basketItemRepository.DeleteBasketItem(new BasketItem
        {
            Id = item.BasketId,
            BasketId = item.BasketItemId,
            ProductVariationId = item.ProductVariationId,
            Stock = item.Stock
        });
    }

    public async Task<BasketPageDto?> GetBasketByCustomerId(int customerId)
    {
        Basket basket = await basketRepository.GetBasketByCustomerId(customerId);

        if (basket == null)
        {
            return null;
        }

        List<BasketItemDto> basketItemDtos = new();

        foreach(var product in basket.BasketItems)
        {
            BasketItemDto basketItemDto = new(
                product.Id,
                product.ProductVariation.Product.Name,
                product.ProductVariation.Product.Image,
                new ProductVariationDto(
                    product.ProductVariation.Id,
                    product.ProductVariation.Product.Id,
                    product.ProductVariation.Category.Name,
                    product.ProductVariation.Price,
                    product.ProductVariation.Volume,
                    product.ProductVariation.Stock
                ),
                product.Stock
            );

            basketItemDtos.Add(basketItemDto);
        }

        return new BasketPageDto(
            Id: basket.Id,
            BasketItems: basketItemDtos
        );
    }

    public async Task<int?> GetBasketItemsCount(int customerId) =>
        await basketItemRepository.GetBasketItemsCount(customerId);
}
