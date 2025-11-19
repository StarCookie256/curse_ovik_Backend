using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.ApplicationLayer.DTO.Brand;
using PerfumeryBackend.ApplicationLayer.DTO.Products;
using PerfumeryBackend.ApplicationLayer.DTO.ProductVariations;
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

    public async Task<List<ProductDto>> GetProductsByBrand(int brandId)
    {
        var products = await productRepository.GetProductsByBrandAsync(brandId);

        List<ProductDto> productDtos = new();

        foreach (var pr in products)
        {
            var varProps = await productVariationService.GetVolumesAndPricesByProductAsync(pr.Id);
            var categories = await productVariationService.GetCategoriesByProductAsync(pr.Id);

            productDtos.Add(new ProductDto(
                Id: pr.Id,
                Name: pr.Name,
                Brand: new BrandDto(Id: pr.Brand.Id, Name: pr.Brand.Name),
                Categories: categories,
                FPrice: varProps.FPrice,
                SPrice: varProps.SPrice,
                FVolume: varProps.FVolume,
                SVolume: varProps.SPrice,
                Gender: pr.Gender,
                Image: pr.Image
            ));
        }

        return productDtos;
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
                _productsOfDay.ProductDtos.Clear();
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
        for (int i = 0; i < 10; i++)
        {
            if(products.Count < 10)
            {
                products.Add(await productRepository.GetProductByIdAsync(random.Next(1, productsCount)));
            }
        }

        List<ProductDto> productDtos = new();

        foreach (var pr in products)
        {
            var varProps = await productVariationService.GetVolumesAndPricesByProductAsync(pr.Id);
            var categories = await productVariationService.GetCategoriesByProductAsync(pr.Id);

            productDtos.Add(new ProductDto(
                Id: pr.Id,
                Name: pr.Name,
                Brand: new BrandDto(Id: pr.Brand.Id, Name: pr.Brand.Name),
                Categories: categories,
                FPrice: varProps.FPrice,
                SPrice: varProps.SPrice,
                FVolume: varProps.FVolume,
                SVolume: varProps.SPrice,
                Gender: pr.Gender,
                Image: pr.Image
            ));
        }

        return new ProductsOfDay(productDtos, DateTime.UtcNow);
    }

    public async Task<PagedResult<ProductDto>> GetProductsBySearch(ProductSearchDto searchDto)
    {
        try
        {
            List<Product> products = await productRepository.GetProductsSearchAsync();

            List<Product> filteredProducts = await ApplyFilters(products, searchDto.ProductFilters);

            int totalCount = filteredProducts.Count;

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
                    Brand: new BrandDto(Id: pr.Brand.Id, Name: pr.Brand.Name),
                    Categories: categories,
                    FPrice: varProps.FPrice,
                    SPrice: varProps.SPrice,
                    FVolume: varProps.FVolume,
                    SVolume: varProps.SVolume,
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetProductsBySearch: {ex.Message}");
            return null;
        }
    }

    private async Task<List<Product>> ApplyFilters(List<Product> products, ProductFiltersDto filters)
    {
        // Фильтр по гендеру
        if (filters.Gender != null && filters.Gender.Count != 0)
        {
            products = products.Where(p => filters.Gender.Contains(p.Gender)).ToList();
        }

        // Фильтр по брендам
        if (filters.Brands != null && filters.Brands.Count != 0)
        {
            products = products
                .Where(p => filters.Brands.Contains(p.Brand.Name))
                .ToList();
        }

        List<ProductVariation> varList = new();

        foreach(var pr in products)
        {
            List<ProductVariation> prVars = await productVariationService.GetVariationsByProductAsync(pr.Id);
            if (prVars != null)
                varList.AddRange(prVars);
        }

        for (int i = products.Count - 1; i >= 0; i--)
        {
            if(products.Contains(products[i]))
                if (!varList.Any(x => x.ProductId == products[i].Id))
                    products.Remove(products[i]);
        }

        // Фильтр по категориям
        if (filters.Categories != null && filters.Categories.Count != 0)
        {
            products = products
                .Where(pr => varList
                    .Where(v => v.ProductId == pr.Id)
                    .Any(v => filters.Categories.Contains(v.Category.Name)))
                .ToList();
            varList = varList
                .Where(v => filters.Categories.Contains(v.Category.Name))
                .ToList();
        }

        // Фильтр по цене
        if (filters.PriceValues != null && filters.PriceValues.Count == 2)
        {
            var minPrice = (double)filters.PriceValues[0];
            var maxPrice = (double)filters.PriceValues[1];

            var filteredByPrice = new List<Product>();
            foreach (var product in products)
            {
                var varProps = await productVariationService.GetVolumesAndPricesByProductAsync(product.Id);
                if (varProps.FPrice >= minPrice && varProps.SPrice <= maxPrice)
                {
                    filteredByPrice.Add(product);
                }
            }
            products = filteredByPrice;
        }

        // Фильтр по объему
        if (filters.VolumeValues != null && filters.VolumeValues.Count == 2)
        {
            var minVolume = (double)filters.VolumeValues[0];
            var maxVolume = (double)filters.VolumeValues[1];

            var filteredByVolume = new List<Product>();
            foreach (var product in products)
            {
                var varProps = await productVariationService.GetVolumesAndPricesByProductAsync(product.Id);
                if (varProps.FVolume >= minVolume && varProps.SVolume <= maxVolume)
                {
                    filteredByVolume.Add(product);
                }
            }
            products = filteredByVolume;
        }

        return products;
    }

    public async Task<ProductPageDto> GetProductForPageByIdAsync(int productId)
    {
        Product product = await productRepository.GetProductForPageByIdAsync(productId);
        if (product == null) return null;

        List<string> categories = await productVariationService.GetCategoriesByProductAsync(product.Id);
        List<ProductVariation> productVariations = await productVariationService.GetVariationsByProductAsync(product.Id);

        List<ProductVariationDto> productVariationDtos = new();
        if (productVariations != null) 
        {
            foreach (var variation in productVariations) 
            {
                productVariationDtos.Add(new ProductVariationDto(
                    variation.Id,
                    product.Id,
                    variation.Category.Name,
                    variation.Price,
                    variation.Volume,
                    variation.Stock));
            }
        }

        ProductPageDto productPageDto = new(
            Id: product.Id,
            Name: product.Name,
            Brand: new BrandDto(Id: product.Brand.Id, Name: product.Brand.Name),
            Categories: categories,
            Gender: product.Gender,
            Image: product.Image,
            Country: product.Country.Name,
            ManufactureYear: product.ManufactureYear,
            ExpirationDate: product.ExpirationDate,
            ProductVariations: productVariationDtos
        );

        return productPageDto;
    }
}
