using Microsoft.AspNetCore.Mvc;
using PerfumeryBackend.ApplicationLayer.DTO.Auth;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.MainLayer.Contracts.Auth;

namespace PerfumeryBackend.MainLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        AccessAndRefreshTokens? success = await authService.Register(new RegisterDto(
            request.Username,
            request.Email,
            request.Password,
            request.Phone));

        if (success == null) 
        {
            return BadRequest("Registration was failed. Try again!");
        }

        Response.Cookies.Append("syndetheite", success.AccessToken);
        return Ok(success);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        AccessAndRefreshTokens? success = await authService.Login(new LoginDto(
            request.Email,
            request.Password));

        if (success == null) 
        { 
            return BadRequest("User not founded. Password or username is incorrect");
        }

        Response.Cookies.Append("syndetheite", success.AccessToken);
        return Ok(success);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        AccessAndRefreshTokens? success = await authService.RefreshToken(new RefreshTokenDto(
            request.RefreshToken));

        if (success == null)
        {
            return BadRequest("Refresh token is expired");
        }

        Response.Cookies.Append("syndetheite", success.AccessToken);

        return Ok(success);
    }
}
