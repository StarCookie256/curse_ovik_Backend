using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByRefreshToken(string refreshToken);
    Task<Customer?> GetByIdAsync(int id);
    Task SetRefreshTokenById(int id, RefreshToken refreshToken);
    Task AddCustomer(Customer customer);
    Task<bool> CustomerAlreadyExist(string customerEmail);
}
