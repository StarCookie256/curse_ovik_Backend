using PerfumeryBackend.ApplicationLayer.DTO.Auth;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using System.Text.Json;
using System.Text;

namespace PerfumeryBackend.MainLayer.Middlewares;

public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtRefreshMiddleware> _logger;
    private readonly string[] _protectedPaths;

    public JwtRefreshMiddleware(RequestDelegate next, ILogger<JwtRefreshMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        // Явно указываем пути, которые требуют JWT проверки
        _protectedPaths = new[]
        {
            "/api/Basket/",
            "/api/Customer/"
        };
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService, IJwtService jwtService)
    {
        var path = context.Request.Path;

        // Проверяем только если путь в списке защищенных
        if (!IsProtectedPath(path))
        {
            await _next(context);
            return;
        }

        var token = GetTokenFromHeader(context);

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token not found");
            return;
        }

        var isValid = await jwtService.ValidateTokenAsync(token);

        if (isValid)
        {
            await _next(context);
            return;
        }

        await HandleTokenRefresh(context, authService);
    }

    private bool IsProtectedPath(PathString path)
    {
        return _protectedPaths.Any(protectedPath => path.StartsWithSegments(protectedPath));
    }

    private string GetTokenFromHeader(HttpContext context)
    {
        return context.Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Replace("Bearer ", "");
    }

    private async Task HandleTokenRefresh(HttpContext context, IAuthService authService)
    {
        try
        {
            var refreshToken = await GetRefreshToken(context);

            if (string.IsNullOrEmpty(refreshToken))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Refresh token not found");
                return;
            }

            var refreshDto = new RefreshTokenDto(refreshToken);
            var refreshResult = await authService.RefreshToken(refreshDto);

            if (refreshResult != null)
            {
                context.Response.Headers["X-New-Access-Token"] = refreshResult.AccessToken;
                context.Response.Headers["X-New-Refresh-Token"] = refreshResult.RefreshToken.Token;
                SetRefreshTokenCookie(context, refreshResult.RefreshToken.Token);
                context.Request.Headers["Authorization"] = $"Bearer {refreshResult.AccessToken}";
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token refresh failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Token refresh error: {ex.Message}");
        }
    }

    private async Task<string?> GetRefreshToken(HttpContext context)
    {
        var refreshTokenFromCookie = context.Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshTokenFromCookie))
            return refreshTokenFromCookie;

        if (context.Request.ContentLength > 0 &&
            context.Request.ContentType?.Contains("application/json") == true)
        {
            context.Request.EnableBuffering();

            try
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(body))
                {
                    var json = JsonSerializer.Deserialize<JsonElement>(body);
                    if (json.TryGetProperty("refreshToken", out var refreshTokenElement))
                    {
                        return refreshTokenElement.GetString();
                    }
                }
            }
            catch
            {
                context.Request.Body.Position = 0;
            }
        }

        return null;
    }

    private void SetRefreshTokenCookie(HttpContext context, string refreshToken)
    {
        context.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"
        });
    }
}