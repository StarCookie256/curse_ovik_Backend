using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeryBackend.ApplicationLayer.DTO.Auth;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace PerfumeryBackend.MainLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpGet("cabinet")]
    public async Task<IActionResult> CustomerCabinet()
    {
        int customerId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value);

        CustomerData? customerData = await customerService.GetCabinet(customerId);

        if (customerData == null) 
        {
            return BadRequest("Customer cabinet by token not found");
        }

        return Ok(customerData);
    }
}
