using PerfumeryBackend.ApplicationLayer.DTO.Auth;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class CustomerService(
    ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<CustomerData?> GetCabinet(int customerId)
    {
        Customer? customer = await customerRepository.GetByIdAsync(customerId);

        if (customer == null)
        {
            return null;
        }

        return new CustomerData(
            Id: customer.Id,
            Image: customer.Image,
            Name: customer.Name,
            Email: customer.Email,
            Phone: customer.Phone,
            Address: customer.Address
        );
    }
}
