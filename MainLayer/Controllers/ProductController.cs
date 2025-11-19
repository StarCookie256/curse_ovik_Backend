using Microsoft.AspNetCore.Mvc;
using PerfumeryBackend.ApplicationLayer.DTO.Products;
using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.MainLayer.Contracts.Product;

namespace PerfumeryBackend.MainLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost("bybrand")]
    public async Task<IActionResult> ProductsByBrand([FromBody] int BrandId)
    {
        List<ProductDto> success = await productService.GetProductsByBrand(BrandId);

        if (success == null)
        {
            return BadRequest("Products getting by brand was failed!");
        }

        return Ok(success);
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagedResult<ProductDto>>> ProductsSearch([FromBody] ProductSearchRequest request)
    {
        PagedResult<ProductDto> success = await productService.GetProductsBySearch(new ProductSearchDto(
            new ProductFiltersDto(
                request.ProductFilters.Gender,
                request.ProductFilters.PriceValues,
                request.ProductFilters.Brands,
                request.ProductFilters.Categories,
                request.ProductFilters.VolumeValues),
            new PaginationDto(
                request.Pagination.Page,
                request.Pagination.PageSize)));

        if (success == null) 
        {
            return BadRequest("Products searching was failed!");
        }

        return Ok(success);
    }

    [HttpGet("ofday")]
    public async Task<IActionResult> ProductsOfDay()
    {
        List<ProductDto> success = await productService.GetProductsOfDayAsync();

        if (success == null)
        {
            return BadRequest("Products searching was failed!");
        }

        return Ok(success);
    }

    [HttpPost("pageinfo")]
    public async Task<IActionResult> ProductPageInfo([FromBody] int productId)
    {
        ProductPageDto success = await productService.GetProductForPageByIdAsync(productId);

        if (success == null)
        {
            return BadRequest("Product page fetch was failed!");
        }

        return Ok(success);
    }
}
