using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IJwtService
{
    public Task<string> GenerateAccessToken(Customer customer);
    public Task<RefreshToken> GenerateRefreshToken();
}
