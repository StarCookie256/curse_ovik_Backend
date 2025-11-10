using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByRefreshToken(string refreshToken);
    Task SetRefreshTokenById(int id, RefreshToken refreshToken);
    Task AddUser(Customer customer);
}
