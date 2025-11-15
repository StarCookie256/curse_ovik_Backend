using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.ApplicationLayer.DTO.Products;
using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class ProductService(
    IProductRepository productRepository,
    IProductVariationService productVariationService) : IProductService
{
    private static ProductsOfDay? _productsOfDay;

    public async Task<List<Product>> GetProductsByBrand(int brandId) =>
        await productRepository.GetProductsByBrandAsync(brandId);

    public Task<Product> GetProductByIdAsync(int productId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ProductDto>> GetProductsOfDayAsync()
    {
        if(_productsOfDay == null)
        {
            _productsOfDay = await GenerateProductsOfDay();

            return _productsOfDay.ProductDtos;
        }
        else
        {
            if (_productsOfDay.LastUpdated < DateTime.UtcNow.AddDays(-1)) 
            {
                _productsOfDay = await GenerateProductsOfDay();

                return _productsOfDay.ProductDtos;
            }
            else
                return _productsOfDay.ProductDtos;
        }
    }

    private async Task<ProductsOfDay> GenerateProductsOfDay()
    {
        List<Product> products = new();
        Random random = new Random();

        int productsCount = await productRepository.GetProductsCountAsync();
        for (int i = 0; i < 11; i++)
        {
            products.Add(await productRepository.GetProductByIdAsync(random.Next(1, productsCount)));
        }

        List<ProductDto> productDtos = new();

        foreach (var pr in products)
        {
            var varProps = await productVariationService.GetVolumesAndPricesByProductAsync(pr.Id);
            var categories = await productVariationService.GetCategoriesByProductAsync(pr.Id);

            productDtos.Add(new ProductDto(
                Id: pr.Id,
                Name: pr.Name,
                Brand: pr.Brand.Name,
                Categories: categories,
                FPrice: varProps.FPrice,
                SPrice: varProps.SPrice,
                FVolume: varProps.FVolume,
                SVolume: varProps.SPrice,
                Gender: pr.Gender,
                Image: pr.Image
            ));
        }

        return new ProductsOfDay(productDtos, DateTime.Now);
    }

    public async Task<PagedResult<ProductDto>> GetProductsBySearch(ProductSearchDto searchDto)
    {
        IQueryable<Product> products = productRepository.GetAllProductsAsync();

        List<Product> filteredProducts = await ApplyFilters(products, searchDto.ProductFilters);

        int totalCount = filteredProducts.Count();

        List<Product> pagedProducts = filteredProducts
            .Skip((searchDto.Pagination.Page - 1) * searchDto.Pagination.PageSize)
            .Take(searchDto.Pagination.PageSize)
            .ToList();

        List<ProductDto> productDtos = new();

        foreach (var pr in pagedProducts)
        {
            var varProps = await productVariationService.GetVolumesAndPricesByProductAsync(pr.Id);
            var categories = await productVariationService.GetCategoriesByProductAsync(pr.Id);

            productDtos.Add(new ProductDto(
                Id: pr.Id,
                Name: pr.Name,
                Brand: pr.Brand.Name,
                Categories: categories,
                FPrice: varProps.FPrice,
                SPrice: varProps.SPrice,
                FVolume: varProps.FVolume,
                SVolume: varProps.SPrice,
                Gender: pr.Gender,
                Image: pr.Image
            ));
        }

        return new PagedResult<ProductDto>
        {
            TotalCount = totalCount,
            Items = productDtos,
            Page = searchDto.Pagination.Page,
            PageSize = searchDto.Pagination.PageSize
        };
    }

    private async Task<List<Product>> ApplyFilters(IQueryable<Product> products, ProductFiltersDto filters)
    {
        // Фильтр по гендеру
        if (filters.Gender != null && filters.Gender.Count != 0)
        {
            products = products.Where(p => filters.Gender.Contains(p.Gender));
        }

        // Фильтр по брендам
        if (filters.Brands != null && filters.Brands.Count != 0)
        {
            products = products
                .Include(p => p.Brand)  // Загружаем связанные данные бренда
                .Where(p => filters.Brands.Contains(p.Brand.Name));
        }

        List<Product> finalProducts = products.ToList();

        List<ProductVariation> varList = new();

        foreach(var finalPr in finalProducts)
        {
            varList.AddRange(await productVariationService.GetVariationsByProductAsync(finalPr.Id));
        }

        // Фильтр по категориям
        if (filters.Categories != null && filters.Categories.Count != 0)
        {
            List<ProductVariation> localVariants = new();
            foreach (var variant in varList)
            {
                if(filters.Categories.Contains(variant.Category.Name))
                    localVariants.Add(variant);
            }

            List<Product> bufferProducts = new();

            foreach (var pr in finalProducts)
            {
                foreach (var variant in localVariants)
                {
                    if(bufferProducts.Count > 0)
                    {
                        if (!bufferProducts.Contains(pr) && variant.ProductId == pr.Id)
                            bufferProducts.Add(pr);
                    }   
                }
            }
            finalProducts = bufferProducts;
            varList = localVariants;
        }

        // фильтруем по цене
        if (filters.PriceValues != null && filters.PriceValues.Count == 2)
        {
            List<Product> bufferProducts = new();
            foreach (var pr in finalProducts)
            {
                var varProps = await productVariationService.GetVolumesAndPricesByProductAsync(pr.Id);
                if(varProps.FPrice <= (double)filters.PriceValues[1] && 
                   varProps.SPrice >= (double)filters.PriceValues[0])
                    bufferProducts.Add(pr);
            }
            finalProducts = bufferProducts;
        }

        // Фильтр по объему
        if (filters.VolumeValues != null && filters.VolumeValues.Count == 2)
        {
            List<Product> bufferProducts = new();
            foreach (var pr in finalProducts)
            {
                var varProps = await productVariationService.GetVolumesAndPricesByProductAsync(pr.Id);
                if (varProps.FVolume <= (double)filters.VolumeValues[1] &&
                   varProps.SVolume >= (double)filters.VolumeValues[0])
                    bufferProducts.Add(pr);
            }
            finalProducts = bufferProducts;
        }

        return finalProducts;
    }
}
