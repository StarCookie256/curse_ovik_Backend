using Microsoft.AspNetCore.Mvc;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.MainLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet("bybrand")]
    public async Task<IActionResult> ProductsByBrand([FromBody] int brandId)
    {
        List<Product> success = await productService.GetProductsByBrand(brandId);

        if (success == null)
        {
            return BadRequest("Products getting by brand was failed!");
        }

        return Ok(success);
    }
}
