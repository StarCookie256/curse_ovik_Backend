using PerfumeryBackend.ApplicationLayer.DTO.Auth;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IAuthService
{
    Task<AccessAndRefreshTokens?> Login(LoginDto loginDto);
    Task<AccessAndRefreshTokens?> Register(RegisterDto registerDto);
    Task<AccessAndRefreshTokens?> RefreshToken(RefreshTokenDto refreshTokenDto);
}
