using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeryBackend.ApplicationLayer.DTO.Basket;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.MainLayer.Contracts.Basket;
using System.IdentityModel.Tokens.Jwt;

namespace PerfumeryBackend.MainLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasketController(IBasketService basketService) : ControllerBase
{
    [HttpGet("bycustomer")]
    public async Task<IActionResult> BasketByCustomer()
    {
        int customerId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value);

        BasketPageDto? basketData = await basketService.GetBasketByCustomerId(customerId);

        if (basketData == null)
        {
            return BadRequest("Basket fetch was failed");
        }

        return Ok(basketData);
    }

    [HttpGet("count")]
    public async Task<IActionResult> BasketItemsCount()
    {
        int customerId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value);

        int? count = await basketService.GetBasketItemsCount(customerId);

        if (count == null) 
        {
            return BadRequest("Basket count fetch was failed");
        }

        return Ok(count);
    }

    [HttpPost("add")]
    public async Task<IActionResult> BasketAddItem([FromBody] BasketRequest request)
    {
        await basketService.AddBasketItem(new BasketDto(
            BasketId: request.BasketId,
            BasketItemId: request.BasketItemId,
            ProductVariationId: request.ProductVariationId,
            Stock: request.Stock
        ));


        return Ok();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> BasketDeleteItem([FromBody] BasketRequest request)
    {
        await basketService.DeleteBasketItem(new BasketDto(
            BasketId: request.BasketId,
            BasketItemId: request.BasketItemId,
            ProductVariationId: request.ProductVariationId,
            Stock: request.Stock
        ));


        return Ok();
    }
}
