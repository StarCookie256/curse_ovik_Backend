using Microsoft.AspNetCore.Mvc;
using PerfumeryBackend.ApplicationLayer.Interfaces;

namespace PerfumeryBackend.MainLayer.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BrandController(IBrandService brandService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> BrandsAll()
    {
        var success = await brandService.GetBrandsAsync();

        if (success == null)
        {
            return BadRequest("Getting all brands was failed!");
        }

        return Ok(success);
    }

    [HttpPost("byname")]
    public async Task<IActionResult> BrandByName([FromBody] string Name)
    {
        var success = await brandService.GetBrandByNameAsync(Name);

        if (success == null)
        {
            return BadRequest("Getting brand by name was failed!");
        }

        return Ok(success);
    }

    [HttpPost("byid")]
    public async Task<IActionResult> BrandById([FromBody] int Id)
    {
        var success = await brandService.GetBrandByIdAsync(Id);

        if (success == null)
        {
            return BadRequest("Getting brand by id was failed!");
        }

        return Ok(success);
    }
}
