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
    }
}
