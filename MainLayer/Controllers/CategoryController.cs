using Microsoft.AspNetCore.Mvc;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.MainLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> CategoriesAll()
    {
        List<Category> success = await categoryService.GetCategoriesAsync();

        if (success == null)
        {
            return BadRequest("Getting all brands was failed!");
        }

        return Ok(success);
    }

    [HttpPost("byid")]
    public async Task<IActionResult> CategoriesById([FromBody] int CategoryId)
    {
        Category success = await categoryService.GetCategoryByIdAsync(CategoryId);

        if (success == null)
        {
            return BadRequest("Getting all brands was failed!");
        }

        return Ok(success);
    }
}
