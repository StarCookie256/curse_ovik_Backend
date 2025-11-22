using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.ApplicationLayer.Entities;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer;    
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class CustomerRepository(PerfumeryDbContext context) : ICustomerRepository
{
    public async Task<IEnumerable<Customer>> GetAllAsync() =>
        await context.Customers
            .AsNoTracking()
            .ToListAsync();

    public async Task<Customer?> GetByEmailAsync(string email) =>
        await context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email);

    public async Task<Customer?> GetByRefreshToken(string refreshToken) =>
        await context.Customers
            .AsNoTracking()
            .Include(x => x.RefreshToken)
            .FirstOrDefaultAsync(x => x.RefreshToken!.Token == refreshToken);

    public async Task SetRefreshTokenById(int id, RefreshToken refreshToken)
    {
        Customer customer = await context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Can not set refresh token to not existing user");

        Customer updatedCustomer = new()
        {
            RefreshToken = refreshToken,
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Password = customer.Password,
            PasswordSalt = customer.PasswordSalt,
            Address = customer.Address,
            Image = customer.Image,
            Phone = customer.Phone,
        };

        context.Customers.Update(updatedCustomer);
        await context.SaveChangesAsync();
    }

    public async Task AddCustomer(Customer customer)
    {
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id) =>
        await context.Customers
            .AsNoTracking()
            .FirstAsync(x => x.Id == id);

    public async Task<bool> CustomerAlreadyExist(string customerEmail) =>
        await context.Customers
            .AsNoTracking()
            .AnyAsync(x => x.Email == customerEmail);
}
