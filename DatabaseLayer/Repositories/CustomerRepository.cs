using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer;    
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class CustomerRepository(PerfumeryDbContext context) : ICustomerRepository
{
    public Task AddUser(Customer customer)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Customer>> GetAllAsync() =>
        await context.Customers
            .AsNoTracking()
            .ToListAsync();

    public async Task<Customer?> GetByIdAsync(int id) =>
        await context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Customer?> GetByRefreshToken(string refreshToken) =>
        await context.Customers
            .AsNoTracking()
            .Include(x => x.RefreshToken)
            .FirstOrDefaultAsync(x => x.RefreshToken!.Token == refreshToken);

    public Task SetRefreshTokenById(int id, RefreshToken refreshToken)
    {
        throw new NotImplementedException();
    }
}
