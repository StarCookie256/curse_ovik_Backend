using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PerfumeryBackend.ApplicationLayer.Services
{
    public class JwtService(IConfiguration configuration) : IJwtService
    {
        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                // Параметры валидации
                var validationParameters = GetTokenValidationParameters();

                // Проверяем токен
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal != null;
            }
            catch (SecurityTokenExpiredException)
            {
                // Токен истек - это "валидный" но просроченный токен
                // Middleware будет пытаться его обновить
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                // Неверная подпись
                return false;
            }
            catch (SecurityTokenMalformedException)
            {
                // Неправильный формат токена
                return false;
            }
            catch (Exception ex)
            {
                // Любая другая ошибка
                Console.WriteLine($"Token validation error: {ex.Message}");
                return false;
            }
        }

        public Task<string> GenerateAccessToken(Customer customer)
        {
            IConfigurationSection jwtSettings = configuration.GetSection("Jwt");
            string keyValue = jwtSettings["Key"]
                ?? throw new Exception("Key value for JWT token was not found!!!");
            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(keyValue));
            SigningCredentials credentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = [
                new Claim(JwtRegisteredClaimNames.Jti, customer.Id.ToString())
            ];

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public Task<RefreshToken> GenerateRefreshToken()
        {
            IConfigurationSection refreshSettings = configuration.GetSection("RefreshToken");
            double expiry = Convert.ToDouble(refreshSettings["ExpiryInMinutes"]
                    ?? throw new Exception("Key value for JWT token was not found!!!"));

            byte[] refreshBytes = new byte[32];
            using RandomNumberGenerator rand = RandomNumberGenerator.Create();
            rand.GetBytes(refreshBytes);

            string token = Convert.ToHexString(refreshBytes);
            DateTime expired = DateTime.UtcNow.AddMinutes(expiry);

            return Task.FromResult(RefreshToken.Create(token, expired));
        }

        public Task<int> GetCustomerIdFromAccessToken(string accessToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            IConfigurationSection jwtSettings = configuration.GetSection("Jwt");
            byte[] key = Encoding.UTF8.GetBytes(jwtSettings["Key"])
                ?? throw new Exception("Key value for JWT token was not found!!!");

            try
            {
                var customerToken = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);


                int customerId = Convert.ToInt32(customerToken.FindFirst(JwtRegisteredClaimNames.Jti)?.Value);
                return Task.FromResult(customerId);
            }
            catch
            {
                return null;
            }
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            IConfigurationSection jwtSettings = configuration.GetSection("Jwt");
            byte[] key = Encoding.UTF8.GetBytes(jwtSettings["Key"])
                ?? throw new Exception("Key value for JWT token was not found!!!");

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],

                ValidateLifetime = true, // Проверяем время жизни токена
                ClockSkew = TimeSpan.Zero // Без допуска по времени
            };
        }
    }
}
